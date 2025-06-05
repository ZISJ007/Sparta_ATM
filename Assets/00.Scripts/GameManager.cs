using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("User Data")]
    // 현재 로그인한 사용자의 UserData 참조
    public UserData userData;

    [Header("User Info UI")]
    public TMP_Text nameText;
    public TMP_Text balanceText;
    public TMP_Text cashText;

    // 내부적으로 UserDatabase를 가지고 있어야, 
    // 로그아웃 혹은 잔액/현금 변경 시 데이터베이스를 업데이트할 수 있음
    private UserDatabase userDatabase;

    // 마지막으로 누가 로그인 했었는지(아이디) 저장하기 위한 키
    private const string LastLoginKey = "LastLoginUserId";

    // 화면에서 UI를 초기화할 때, 이전에 저장된 값을 비교하기 위한 변수
    private int lastBalance;
    private int lastCash;

    private void Awake()
    {
        // 싱글톤 패턴 (씬 전환 시에도 유지)
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

        // 1) 전체 회원 정보를 PlayerPrefs에서 불러옴
        userDatabase = UserDatabase.LoadFromPlayerPrefs();

        // 2) “마지막 로그인 ID”가 남아 있으면 → UserDatabase에서 찾아서 현재 userData로 설정
        if (PlayerPrefs.HasKey(LastLoginKey))
        {
            string lastId = PlayerPrefs.GetString(LastLoginKey);
            UserData found = FindUserInDatabase(lastId);
            if (found != null)
            {
                // 해당 계정으로 로그인 상태를 복원
                userData = found;
            }
            else
            {
                // UserDatabase에는 해당 ID가 없으면 기본값 처리하거나 null로 두기
                userData = null;
            }
        }

        // 3) 만약 userData가 null(즉, 최초 실행이거나, 이전에 로그인 정보가 유효하지 않았던 경우)
        if (userData == null)
        {
            // 기본값을 지정하고 싶으면 아래처럼 쓰되, 
            // 이 경우 balance/cash가 자동으로 저장될 수 있으니 주의
            userData = new UserData("guest", "", "방문객", balance: 0, cash: 0);
        }

        // 4) UI 갱신을 위해 이전 값 저장
        lastBalance = userData.balance;
        lastCash = userData.cash;
    }

    private void Update()
    {
        UpdateUI();

        // 잔액이나 현금이 변동되었을 때 → UserDatabase에도 반영하고 저장
        if (userData.balance != lastBalance || userData.cash != lastCash)
        {
            // 1) UserDatabase에서 해당 userData를 찾아서 업데이트
            UpdateUserInDatabase(userData);

            // 2) UserDatabase를 다시 PlayerPrefs에 저장
            userDatabase.SaveToPlayerPrefs();

            // 3) 현재 userData 값도 별도로 PlayerPrefs(“UserData” 키)로 저장하고 싶으면 아래 주석 해제
            // SaveUserData(); 

            lastBalance = userData.balance;
            lastCash = userData.cash;
        }
    }

    /// <summary>
    /// 화면에 사용자 이름/잔액/현금 보여주는 부분
    /// </summary>
    public void UpdateUI()
    {
        if (userData == null)
        {
            nameText.text = "로그인되지 않음";
            balanceText.text = $"잔액: -";
            cashText.text = $"현금: -";
            return;
        }

        nameText.text = $"{userData.name}";
        balanceText.text = $"Balance\t{userData.balance:N0}원";
        cashText.text = $"현금 \n{userData.cash:N0}";
    }

    /// <summary>
    /// 입금 기능 (AuthManager에서 로그인된 userData에 접근하여 호출할 수 있음)
    /// </summary>
    public void Deposit(int amount)
    {
        if (userData == null) return;

        userData.balance += amount;
        userData.cash -= amount;
        // 이후 Update()에서 database 저장 로직이 자동으로 실행됨
    }

    /// <summary>
    /// 출금 기능
    /// </summary>
    public void Withdraw(int amount)
    {
        if (userData == null) return;

        userData.balance -= amount;
        userData.cash += amount;
        // 이후 Update()에서 database 저장 로직이 자동으로 실행됨
    }

    /// <summary>
    /// (선택) 필요 시, UserData 전체를 “UserData” 키로 단일 JSON 저장
    /// </summary>
    public void SaveUserData()
    {
        if (userData == null) return;

        string json = userData.ToJson();
        PlayerPrefs.SetString("UserData", json);
        PlayerPrefs.Save();
        Debug.Log("데이터 저장됨 (UserData 키): " + json);
    }

    /// <summary>
    /// PlayerPrefs → JSON에서 UserData 복원 (단일 저장용)
    /// </summary>
    public void LoadUserData()
    {
        if (PlayerPrefs.HasKey("UserData"))
        {
            string json = PlayerPrefs.GetString("UserData");
            userData = UserData.FromJson(json);
            Debug.Log("데이터 불러옴 (UserData 키): " + json);
        }
    }

    /// <summary>
    /// UserDatabase 리스트를 순회하며, userId가 일치하는 UserData를 반환
    /// </summary>
    private UserData FindUserInDatabase(string userId)
    {
        foreach (UserData u in userDatabase.users)
        {
            if (u.userId.Equals(userId))
                return u;
        }
        return null;
    }

    /// <summary>
    /// UserDatabase 리스트에서 인자로 들어온 UserData.userId와 같은 항목을 찾아, 
    /// 해당 리스트의 인덱스를 수정하여 최신 userData 값으로 교체
    /// </summary>
    private void UpdateUserInDatabase(UserData updatedUser)
    {
        for (int i = 0; i < userDatabase.users.Count; i++)
        {
            if (userDatabase.users[i].userId.Equals(updatedUser.userId))
            {
                // 참조형이므로 userDatabase.users[i] 자체를 직접 덮어써도 되고,
                // 필요한 필드만 복사해도 무방합니다. 예를 들어:
                userDatabase.users[i].balance = updatedUser.balance;
                userDatabase.users[i].cash = updatedUser.cash;
                // (이 외에 name이나 비밀번호가 변경될 수 있다면, 같은 방식으로 필드 복사)
                break;
            }
        }
    }

    /// <summary>
    /// AuthManager에서 로그인 성공 시 호출하도록 공개한 메서드
    /// </summary>
    public void SetCurrentUser(UserData loggedInUser)
    {
        userData = loggedInUser;

        // UI를 즉시 한번 업데이트
        UpdateUI();

        // 잔액/현금 비교를 위해 lastBalance, lastCash 초기화
        lastBalance = userData.balance;
        lastCash = userData.cash;
    }

    /// <summary>
    /// 로그아웃 기능(선택)  
    /// - PlayerPrefs에 저장된 LastLoginUserId 정보를 삭제  
    /// - userData를 null 또는 guest로 초기화  
    /// - UI도 ‘로그인되지 않음’으로 바꾸려면 이 메서드를 호출하세요.
    /// </summary>
    public void Logout()
    {
        PlayerPrefs.DeleteKey(LastLoginKey);
        PlayerPrefs.Save();

        userData = null;
        UpdateUI();
    }
}
