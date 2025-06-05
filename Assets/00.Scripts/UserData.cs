using UnityEngine;


[System.Serializable]
public class UserData
{
    [Header("User Info")]
    public string name = "";
    public int balance = 0;
    public int cash = 0;

    public UserData(string name, int balance, int cash)
    {
        this.name = name;
        this.balance = balance;
        this.cash = cash;
    }
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static UserData FromJson(string json)
    {
        return JsonUtility.FromJson<UserData>(json);
    }
}
