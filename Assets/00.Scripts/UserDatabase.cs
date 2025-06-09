using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserDatabase
{
    // ������ ȸ�������� ���� List. UserData �ϳ��� �� ȸ���� �ǹ�.
    public List<UserData> users = new List<UserData>();

    // PlayerPrefs�� ������ �� ����ϴ� Ű
    private const string DatabaseKey = "UserDatabase";

    // ���� �޸𸮻��� ������(users ����Ʈ)�� JSON���� ��ȯ�� �� PlayerPrefs�� ����
    public void SaveToPlayerPrefs()
    {
        // JsonUtility�� ��ü ��ü�� ����ȭ
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(DatabaseKey, json);
        PlayerPrefs.Save(); // ��� ����
        Debug.Log($"[UserDatabase] ������ �����: {json}");
    }

    // PlayerPrefs�� ����� JSON�� �ҷ��� UserDatabase ��ü�� �����ϰų�,  
    // ����� ������ ���ٸ� ��� �ִ� �� �����ͺ��̽� ��ü�� ��ȯ
    public static UserDatabase LoadFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(DatabaseKey))
        {
            string json = PlayerPrefs.GetString(DatabaseKey);
            Debug.Log($"[UserDatabase] ������ �ҷ���: {json}");
            return JsonUtility.FromJson<UserDatabase>(json);
        }
        else
        {
            // ����� �����Ͱ� ������ ���� ����
            return new UserDatabase();
        }
    }
}
