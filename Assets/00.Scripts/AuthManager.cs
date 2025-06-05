using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 회원가입과 로그인 로직을 담당하는 매니저.  
/// UserDatabase(=여러 회원의 정보) 를 Load/Save하여,  
/// 새 회원 가입 시 List에 추가하고, 로그인 시 일치하는 정보가 있는지 체크.
/// </summary>
public class AuthManager : MonoBehaviour
{
    [Header("---- 회원가입 UI ----")]
    [Tooltip("회원가입 입력: 아이디")]
    public TMP_InputField signupUserIdInput;
    [Tooltip("회원가입 입력: 비밀번호")]
    public TMP_InputField signupPasswordInput;
    [Tooltip("회원가입 입력: 이름")]
    public TMP_InputField signupNameInput;
    [Tooltip("회원가입 후 상태 메세지 출력용")]
    public TMP_Text signupFeedbackText;
    public GameObject signupPanel; // 회원가입 패널
    [Space(10)]
    [Header("---- 로그인 UI ----")]
    [Tooltip("로그인 입력: 아이디")]
    public TMP_InputField loginUserIdInput;
    [Tooltip("로그인 입력: 비밀번호")]
    public TMP_InputField loginPasswordInput;
    [Tooltip("로그인 후 상태 메세지 출력용")]
    public TMP_Text loginFeedbackText;
    public GameObject loginPanel; // 회원가입 패널
    [Space(10)]
    [Header("---- ATM UI ----")]
    public GameObject atmPanel;

    // 실제로 회원정보가 저장된 데이터베이스
    private UserDatabase userDatabase;

    // PlayerPrefs에 저장할 “마지막 로그인 ID” 키
    private const string LastLoginKey = "LastLoginUserId";
    private void Awake()
    {
        // 씬이 시작될 때 PlayerPrefs에 저장된 DB가 있으면 불러오고, 없으면 새로 생성
        userDatabase = UserDatabase.LoadFromPlayerPrefs();
    }

    /// <summary>
    /// '회원가입' 버튼을 눌렀을 때 호출될 메서드
    /// </summary>
    public void OnClick_Signup()
    {
        string userId = signupUserIdInput.text.Trim();
        string password = signupPasswordInput.text.Trim();
        string name = signupNameInput.text.Trim();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
        {
            signupFeedbackText.text = "<color=red>모든 항목을 입력해주세요.</color>";
            return;
        }

        if (IsUserIdTaken(userId))
        {
            signupFeedbackText.text = "<color=red>이미 존재하는 아이디입니다.</color>";
            return;
        }

        // 신규 회원 생성 (초기 잔고는 0, 현금도 0)
        UserData newUser = new UserData(userId, password, name, balance: 0, cash: 0);

        // 데이터베이스 리스트에 추가
        userDatabase.users.Add(newUser);
        // 즉시 저장
        userDatabase.SaveToPlayerPrefs();

        signupFeedbackText.text = "<color=green>회원가입 성공! 로그인 후 이용하세요.</color>";
        signupUserIdInput.text = "";
        signupPasswordInput.text = "";
        signupNameInput.text = "";
    }

    /// <summary>
    /// '로그인' 버튼을 눌렀을 때 호출될 메서드
    /// </summary>
    public void OnClick_Login()
    {
        string userId = loginUserIdInput.text.Trim();
        string password = loginPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            loginFeedbackText.text = "<color=red>아이디와 비밀번호를 입력해주세요.</color>";
            return;
        }

        UserData found = FindUser(userId, password);
        if (found == null)
        {
            loginFeedbackText.text = "<color=red>아이디 또는 비밀번호가 잘못되었습니다.</color>";
            return;
        }

        // 로그인 성공 → GameManager 의 userData 에 할당
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentUser(found);
            loginPanel.SetActive(false); // 로그인 패널 닫기
            atmPanel.SetActive(true); // ATM 패널 열기
        }

        // “마지막 로그인된 사용자 ID”를 PlayerPrefs에 저장
        PlayerPrefs.SetString(LastLoginKey, userId);
        PlayerPrefs.Save();

        loginFeedbackText.text = $"<color=green>{found.name}님, 환영합니다!</color>";

        // 원한다면 여기서 메인 게임 씬으로 전환
        // SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// 같은 아이디가 이미 존재하는지 여부를 검사한다.
    /// </summary>
    /// <param name="userId">검사할 아이디</param>
    /// <returns>이미 존재하면 true, 없으면 false</returns>
    private bool IsUserIdTaken(string userId)
    {
        foreach (UserData u in userDatabase.users)
        {
            if (u.userId.Equals(userId))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 아이디·비밀번호가 일치하는 회원이 있으면 그 UserData 객체를 반환하고,  
    /// 없으면 null을 반환한다.
    /// </summary>
    /// <param name="userId">검색할 아이디</param>
    /// <param name="password">검색할 비밀번호</param>
    private UserData FindUser(string userId, string password)
    {
        foreach (UserData u in userDatabase.users)
        {
            if (u.userId.Equals(userId) && u.password.Equals(password))
                return u;
        }
        return null;
    }
    /// <summary>
    /// 다른 곳에서 UserDatabase 업데이트 시(예: 잔액/현금 변경 등), 
    /// AuthManager 쪽에서도 다시 불러오고 싶다면 호출할 수 있음
    /// </summary>
    public void ReloadUserDatabase()
    {
        userDatabase = UserDatabase.LoadFromPlayerPrefs();
    }
    public void OnClick_SignupPanel()
    {
        signupPanel.SetActive(true);
        loginPanel.SetActive(false);
    }
    public void OnClick_LoginPanel()
    {
        signupPanel.SetActive(false);
        loginPanel.SetActive(true);
    }
}
