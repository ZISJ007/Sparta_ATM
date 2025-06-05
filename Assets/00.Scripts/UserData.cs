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

    /// <summary>
    /// ȸ������ �� userId, ��й�ȣ, �̸�, �ʱ� balance��cash�� �ѹ��� �����ϰ� ���� ��� ����� ������
    /// </summary>
    /// <param name="userId">ȸ�� ���̵�</param>
    /// <param name="password">ȸ�� ��й�ȣ</param>
    /// <param name="name">ȸ�� �̸�</param>
    /// <param name="balance">�ʱ� ���� �ܰ�</param>
    /// <param name="cash">�ʱ� ���� �ܰ�</param>
    public UserData(string userId, string password, string name, int balance, int cash)
    {
        this.userId = userId;
        this.password = password;
        this.name = name;
        this.balance = balance;
        this.cash = cash;
    }

    /// <summary>
    /// ������ �����ֽ� ������: �̸����ܰ����ݸ� ������ �� (��: ���� ����� ��� ��)
    /// </summary>
    /// <param name="name">ȸ�� �̸�</param>
    /// <param name="balance">�ʱ� ���� �ܰ�</param>
    /// <param name="cash">�ʱ� ���� �ܰ�</param>
    public UserData(string name, int balance, int cash)
    {
        this.name = name;
        this.balance = balance;
        this.cash = cash;
    }

    /// <summary>
    /// �� ��ü�� JSON ���ڿ��� ��ȯ (PlayerPrefs � �����ϱ� ����)
    /// </summary>
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    /// <summary>
    /// JSON ���ڿ����� UserData ��ü�� ����
    /// </summary>
    /// <param name="json">JsonUtility.ToJson ������ ���ڿ�</param>
    public static UserData FromJson(string json)
    {
        return JsonUtility.FromJson<UserData>(json);
    }
}
