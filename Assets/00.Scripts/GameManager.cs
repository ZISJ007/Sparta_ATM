using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("※ 로그인된 사용자 데이터")]
    public UserData userData;       // 현재 로그인된 유저의 UserData

    [Header("※ User Info UI (현재 로그인된 사용자 정보 화면)")]
    public TMP_Text nameText;       // 사용자 이름 표시용
    public TMP_Text balanceText;    // 잔액 표시용
    public TMP_Text cashText;       // 현금 표시용

    // 내부적으로 전체 회원 정보를 관리하는 UserDatabase
    private UserDatabase userDatabase;

    // PlayerPrefs에 마지막으로 로그인했던 사용자 ID를 저장하기 위한 키
    private const string LastLoginKey = "LastLoginUserId";

    // UI 갱신 시 이전 값과 비교하기 위한 보조 변수
    private int lastBalance;
    private int lastCash;

    private void Awake()
    {
        // 싱글톤 패턴 유지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        // PlayerPrefs로부터 전체 회원(UserDatabase) 불러오기 
        userDatabase = UserDatabase.LoadFromPlayerPrefs();

        // 마지막 로그인했던 사용자 ID가 있는지 확인하고, 있으면 자동 로그인 
        if (PlayerPrefs.HasKey(LastLoginKey))
        {
            string lastId = PlayerPrefs.GetString(LastLoginKey);
            UserData found = FindUserInDatabase(lastId);
            if (found != null)
            {
                // 자동으로 로그인 상태 복원
                userData = found;
            }
            else
            {
                // DB에는 없는데 LastLoginKey만 남아있는 경우 (비정상)
                userData = null;
            }
        }

        // userData가 null(비로그인 상태)이면, guest 계정으로 초기화하거나, null 그대로 두기 ─
        if (userData == null)
        {
            // “방문객(guest)” 계정 예시 → guest 계정에는 비밀번호가 없으므로, ID만 "guest"로 해 둡니다.
            userData = new UserData("guest", "", "방문객", balance: 0, cash: 0);
        }

        // UI에 표시할 잔액/현금 초기값 백업 
        lastBalance = userData.balance;
        lastCash = userData.cash;
    }

    private void Update()
    {
        // 매 프레임 UI 업데이트 
        UpdateUI();

        // 잔액/현금 변화 감지
        if (userData.balance != lastBalance || userData.cash != lastCash)
        {
            // 바뀐 사용자 정보를 UserDatabase에 반영 후 저장
            UpdateUserInDatabase(userData);
            userDatabase.SaveToPlayerPrefs();

            // 보조 변수 갱신
            lastBalance = userData.balance;
            lastCash = userData.cash;
        }
    }

    // 화면 UI에 현재 로그인된 사용자 이름, 잔액, 현금을 표시
    // 비로그인 상태(guest 등)일 땐 “로그인되지 않음”으로 표기

    public void UpdateUI()
    {
        if (userData == null || userData.userId == "guest")
        {
            // guest 계정이거나 null인 경우
            nameText.text = "로그인되지 않음";
            balanceText.text = $"잔액: -";
            cashText.text = $"현금: -";
            return;
        }

        // 유효한 로그인 상태
        nameText.text = $"{userData.name}";
        balanceText.text = $"Balance\t{userData.balance:N0}원";
        cashText.text = $"현금 \n{userData.cash:N0}";
    }

    // 입금: 현금을 줄이고, 잔액을 늘린다.
    // 금액이 음수거나 현금이 부족할 경우, 아무 동작 안 함.
    public void Deposit(int amount)
    {
        if (userData == null || amount <= 0) return;
        if (userData.cash < amount) return; // 현금이 부족하면 무시

        userData.balance += amount;
        userData.cash -= amount;
        // Update()에서 변화 감지 → UserDatabase에 저장 → UI 갱신
    }

    // 출금: 잔액을 줄이고, 현금을 늘린다.
    // 금액이 음수거나 잔액이 부족할 경우, 아무 동작 안 함.
    public void Withdraw(int amount)
    {
        if (userData == null || amount <= 0) return;
        if (userData.balance < amount) return; // 잔액이 부족하면 무시

        userData.balance -= amount;
        userData.cash += amount;
        // Update()에서 변화 감지 → UserDatabase에 저장 → UI 갱신
    }


    public bool TransferTo(string recipientIdOrName, int amount, out string errorMessage)
    {
        errorMessage = "";

        // 로그인 상태 체크
        if (userData == null || userData.userId == "guest")
        {
            errorMessage = "로그인된 사용자 정보를 찾을 수 없습니다.";
            return false;
        }

        // 금액 유효성 검사
        if (amount <= 0)
        {
            errorMessage = "송금할 금액을 1 이상 입력해주세요.";
            return false;
        }

        if (userData.balance < amount)
        {
            errorMessage = "잔액이 부족합니다.";
            return false;
        }

        // recipientIdOrName 비어 있는지 검사
        if (string.IsNullOrEmpty(recipientIdOrName))
        {
            errorMessage = "수신자(ID 또는 이름)를 입력해주세요.";
            return false;
        }

        // 수신자 찾기 (ID 우선, 없으면 이름으로 검색)
        UserData recipient = null;

        // userId 비교
        recipient = FindUserInDatabase(recipientIdOrName.Trim());

        // ID와 일치하는 회원이 없으면, 이름으로 검색
        if (recipient == null)
        {
            // 이름으로 검색 (case-sensitive, 필요한 경우 ToLower / ToUpper로 통일)
            foreach (UserData u in userDatabase.users)
            {
                if (u.name.Equals(recipientIdOrName.Trim()))
                {
                    recipient = u;
                    break;
                }
            }
        }

        if (recipient == null)
        {
            errorMessage = "입력하신 수신자(ID 또는 이름)를 찾을 수 없습니다.";
            return false;
        }

        // 자기 자신에게 송금하지 못하게 체크
        if (recipient.userId.Equals(userData.userId))
        {
            errorMessage = "자기 자신에게는 송금할 수 없습니다.";
            return false;
        }

        // 모든 검사를 통과했으므로, 실제로 송금 로직 수행
        // 내 계좌(userData)의 balance 차감
        // 상대 계좌(recipient)의 balance 증가
        userData.balance -= amount;
        recipient.balance += amount;

        // UserDatabase 리스트에 반영
        UpdateUserInDatabase(userData);
        UpdateUserInDatabase(recipient);

        // PlayerPrefs에 UserDatabase 재저장
        userDatabase.SaveToPlayerPrefs();

        // 오류 메시지 비우고(true 반환)
        return true;
    }

    // UserDatabase 리스트를 순회하며, 
    // userId와 정확히 일치하는 UserData를 반환. 
    // 없으면 null 반환.
    private UserData FindUserInDatabase(string userId)
    {
        foreach (UserData u in userDatabase.users)
        {
            if (u.userId.Equals(userId))
                return u;
        }
        return null;
    }

    // UserDatabase 리스트에서, 인자로 들어온 UserData.userId와 동일한 항목을 찾아서 
    // balance/cash/name/password 등의 필드를 동기화(덮어쓰기)한다. 
    private void UpdateUserInDatabase(UserData updatedUser)
    {
        for (int i = 0; i < userDatabase.users.Count; i++)
        {
            if (userDatabase.users[i].userId.Equals(updatedUser.userId))
            {
                // 실제로 교체할 필드만 덮어쓰기
                userDatabase.users[i].balance = updatedUser.balance;
                userDatabase.users[i].cash = updatedUser.cash;
                userDatabase.users[i].name = updatedUser.name;
                userDatabase.users[i].password = updatedUser.password;
                break;
            }
        }
    }


    // AuthManager에서 로그인에 성공하면 호출 
    // → 현재 로그인된 userData를 설정하고, UI를 즉시 갱신
    public void SetCurrentUser(UserData loggedInUser)
    {
        userData = loggedInUser;

        // UI 즉시 갱신
        UpdateUI();

        // 보조 변수 초기화
        lastBalance = userData.balance;
        lastCash = userData.cash;
    }


    public void SaveUserData()
    {
        if (userData == null) return;

        string json = userData.ToJson();
        PlayerPrefs.SetString("UserData", json);
        PlayerPrefs.Save();
        Debug.Log("데이터 저장됨 (UserData 키): " + json);
    }

    public void LoadUserData()
    {
        if (PlayerPrefs.HasKey("UserData"))
        {
            string json = PlayerPrefs.GetString("UserData");
            userData = UserData.FromJson(json);
            Debug.Log("데이터 불러옴 (UserData 키): " + json);
        }
    }
}
