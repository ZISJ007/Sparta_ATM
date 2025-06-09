using UnityEngine;

[System.Serializable]
public class UserData
{
    [Header("User Info")]
    public string userId = "";    // ���� ID (�α��� �� ���)
    public string password = "";  // ��й�ȣ (�α��� �� ���)
    public string name = "";      // ���� �̸� �Ǵ� ǥ�� �̸�

    [Header("Account Data")]
    public int balance = 0;       // ���� �ܰ�
    public int cash = 0;          // ���� �ܰ�

    // ȸ������ �� userId, ��й�ȣ, �̸�, �ʱ� balance��cash�� �ѹ��� �����ϰ� ���� ��� ����� ������
    public UserData(string userId, string password, string name, int balance, int cash)
    {
        this.userId = userId;
        this.password = password;
        this.name = name;
        this.balance = balance;
        this.cash = cash;
    }
    public UserData(string name, int balance, int cash)
    {
        this.name = name;
        this.balance = balance;
        this.cash = cash;
    }

    // �� ��ü�� JSON ���ڿ��� ��ȯ (PlayerPrefs � �����ϱ� ����)
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    /// JSON ���ڿ����� UserData ��ü�� ����
    public static UserData FromJson(string json)
    {
        return JsonUtility.FromJson<UserData>(json);
    }
}
