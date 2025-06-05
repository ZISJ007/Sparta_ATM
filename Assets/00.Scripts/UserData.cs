using UnityEngine;

[System.Serializable]
public class UserData
{
    [Header("User Info")]
    public string userId = "";    // 유저 ID (로그인 시 사용)
    public string password = "";  // 비밀번호 (로그인 시 사용)
    public string name = "";      // 실제 이름 또는 표시 이름

    [Header("Account Data")]
    public int balance = 0;       // 은행 잔고
    public int cash = 0;          // 현금 잔고

    /// <summary>
    /// 회원가입 시 userId, 비밀번호, 이름, 초기 balance·cash를 한번에 설정하고 싶은 경우 사용할 생성자
    /// </summary>
    /// <param name="userId">회원 아이디</param>
    /// <param name="password">회원 비밀번호</param>
    /// <param name="name">회원 이름</param>
    /// <param name="balance">초기 은행 잔고</param>
    /// <param name="cash">초기 현금 잔고</param>
    public UserData(string userId, string password, string name, int balance, int cash)
    {
        this.userId = userId;
        this.password = password;
        this.name = name;
        this.balance = balance;
        this.cash = cash;
    }

    /// <summary>
    /// 기존에 보내주신 생성자: 이름·잔고·현금만 설정할 때 (예: 단일 사용자 모드 등)
    /// </summary>
    /// <param name="name">회원 이름</param>
    /// <param name="balance">초기 은행 잔고</param>
    /// <param name="cash">초기 현금 잔고</param>
    public UserData(string name, int balance, int cash)
    {
        this.name = name;
        this.balance = balance;
        this.cash = cash;
    }

    /// <summary>
    /// 이 객체를 JSON 문자열로 변환 (PlayerPrefs 등에 저장하기 위해)
    /// </summary>
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    /// <summary>
    /// JSON 문자열에서 UserData 객체를 생성
    /// </summary>
    /// <param name="json">JsonUtility.ToJson 형식의 문자열</param>
    public static UserData FromJson(string json)
    {
        return JsonUtility.FromJson<UserData>(json);
    }
}
