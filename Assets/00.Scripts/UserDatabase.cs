using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserDatabase
{
    /// <summary>
    /// 실제로 회원정보를 담을 List. UserData 하나가 한 회원을 의미.
    /// </summary>
    public List<UserData> users = new List<UserData>();

    // PlayerPrefs에 저장할 때 사용하는 키
    private const string DatabaseKey = "UserDatabase";

    /// <summary>
    /// 현재 메모리상의 데이터(users 리스트)를 JSON으로 변환한 뒤 PlayerPrefs에 저장
    /// </summary>
    public void SaveToPlayerPrefs()
    {
        // JsonUtility는 객체 전체를 직렬화
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(DatabaseKey, json);
        PlayerPrefs.Save(); // 즉시 저장
        Debug.Log($"[UserDatabase] 데이터 저장됨: {json}");
    }

    /// <summary>
    /// PlayerPrefs에 저장된 JSON을 불러와 UserDatabase 객체로 복원하거나,  
    /// 저장된 정보가 없다면 비어 있는 새 데이터베이스 객체를 반환
    /// </summary>
    public static UserDatabase LoadFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(DatabaseKey))
        {
            string json = PlayerPrefs.GetString(DatabaseKey);
            Debug.Log($"[UserDatabase] 데이터 불러옴: {json}");
            return JsonUtility.FromJson<UserDatabase>(json);
        }
        else
        {
            // 저장된 데이터가 없으면 새로 생성
            return new UserDatabase();
        }
    }
}
