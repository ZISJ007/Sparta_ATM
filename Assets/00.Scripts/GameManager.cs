using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("User Data")]
    public UserData userData;

    [Header("User Info UI")]
    public TMP_Text nameText;
    public TMP_Text balanceText;
    public TMP_Text cashText;

    private int lastBalance;
    private int lastCash;
    private const string UserDataKey = "UserData";
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadUserData();

        if (userData == null)
            userData = new UserData("지승준", 10000, 50000);

        // 🔄 마지막 저장값 초기화
        lastBalance = userData.balance;
        lastCash = userData.cash;
    }
    void Update()
    {
        UpdateUI();
        if (userData.balance != lastBalance || userData.cash != lastCash)
        {
            SaveUserData();

            // 최신 값으로 갱신
            lastBalance = userData.balance;
            lastCash = userData.cash;
        }
    }
    public void Deposit(int amount)
    {
        
            userData.balance += amount; // 잔액에 추가
            userData.cash -= amount; // 현금에서 차감
    }
    public void Withdraw(int amount)
    {
            userData.balance -= amount; // 잔액에서 차감
            userData.cash += amount; // 현금에서 추가
    }

    public void UpdateUI()
    {
        nameText.text = $"{userData.name}";
        balanceText.text = $"Banlance\t{userData.balance:N0}원";
        cashText.text = $"현금 \n{userData.cash:N0}";
    }
    public void SaveUserData()
    {
        string json = userData.ToJson();
        PlayerPrefs.SetString(UserDataKey, json);
        PlayerPrefs.Save();
        Debug.Log("데이터 저장됨: " + json);
    }

    public void LoadUserData()
    {
        if (PlayerPrefs.HasKey(UserDataKey))
        {
            string json = PlayerPrefs.GetString(UserDataKey);
            userData = UserData.FromJson(json);
            Debug.Log("데이터 불러옴: " + json);
        }
    }

    // 필요하면 리셋도 가능
    public void ResetUserData()
    {
        PlayerPrefs.DeleteKey(UserDataKey);
        userData = new UserData("지승준", 10000, 50000);
        SaveUserData();
    }
}
