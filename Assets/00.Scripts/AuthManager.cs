using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// ȸ�����԰� �α��� ������ ����ϴ� �Ŵ���.  
/// UserDatabase(=���� ȸ���� ����) �� Load/Save�Ͽ�,  
/// �� ȸ�� ���� �� List�� �߰��ϰ�, �α��� �� ��ġ�ϴ� ������ �ִ��� üũ.
/// </summary>
public class AuthManager : MonoBehaviour
{
    [Header("---- ȸ������ UI ----")]
    [Tooltip("ȸ������ �Է�: ���̵�")]
    public TMP_InputField signupUserIdInput;
    [Tooltip("ȸ������ �Է�: ��й�ȣ")]
    public TMP_InputField signupPasswordInput;
    [Tooltip("ȸ������ �Է�: �̸�")]
    public TMP_InputField signupNameInput;
    [Tooltip("ȸ������ �� ���� �޼��� ��¿�")]
    public TMP_Text signupFeedbackText;
    public GameObject signupPanel; // ȸ������ �г�
    [Space(10)]
    [Header("---- �α��� UI ----")]
    [Tooltip("�α��� �Է�: ���̵�")]
    public TMP_InputField loginUserIdInput;
    [Tooltip("�α��� �Է�: ��й�ȣ")]
    public TMP_InputField loginPasswordInput;
    [Tooltip("�α��� �� ���� �޼��� ��¿�")]
    public TMP_Text loginFeedbackText;
    public GameObject loginPanel; // ȸ������ �г�
    [Space(10)]
    [Header("---- ATM UI ----")]
    public GameObject atmPanel;

    // ������ ȸ�������� ����� �����ͺ��̽�
    private UserDatabase userDatabase;

    // PlayerPrefs�� ������ �������� �α��� ID�� Ű
    private const string LastLoginKey = "LastLoginUserId";
    private void Awake()
    {
        // ���� ���۵� �� PlayerPrefs�� ����� DB�� ������ �ҷ�����, ������ ���� ����
        userDatabase = UserDatabase.LoadFromPlayerPrefs();
    }

    /// <summary>
    /// 'ȸ������' ��ư�� ������ �� ȣ��� �޼���
    /// </summary>
    public void OnClick_Signup()
    {
        string userId = signupUserIdInput.text.Trim();
        string password = signupPasswordInput.text.Trim();
        string name = signupNameInput.text.Trim();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
        {
            signupFeedbackText.text = "<color=red>��� �׸��� �Է����ּ���.</color>";
            return;
        }

        if (IsUserIdTaken(userId))
        {
            signupFeedbackText.text = "<color=red>�̹� �����ϴ� ���̵��Դϴ�.</color>";
            return;
        }

        // �ű� ȸ�� ���� (�ʱ� �ܰ�� 0, ���ݵ� 0)
        UserData newUser = new UserData(userId, password, name, balance: 0, cash: 0);

        // �����ͺ��̽� ����Ʈ�� �߰�
        userDatabase.users.Add(newUser);
        // ��� ����
        userDatabase.SaveToPlayerPrefs();

        signupFeedbackText.text = "<color=green>ȸ������ ����! �α��� �� �̿��ϼ���.</color>";
        signupUserIdInput.text = "";
        signupPasswordInput.text = "";
        signupNameInput.text = "";
    }

    /// <summary>
    /// '�α���' ��ư�� ������ �� ȣ��� �޼���
    /// </summary>
    public void OnClick_Login()
    {
        string userId = loginUserIdInput.text.Trim();
        string password = loginPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            loginFeedbackText.text = "<color=red>���̵�� ��й�ȣ�� �Է����ּ���.</color>";
            return;
        }

        UserData found = FindUser(userId, password);
        if (found == null)
        {
            loginFeedbackText.text = "<color=red>���̵� �Ǵ� ��й�ȣ�� �߸��Ǿ����ϴ�.</color>";
            return;
        }

        // �α��� ���� �� GameManager �� userData �� �Ҵ�
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentUser(found);
            loginPanel.SetActive(false); // �α��� �г� �ݱ�
            atmPanel.SetActive(true); // ATM �г� ����
        }

        // �������� �α��ε� ����� ID���� PlayerPrefs�� ����
        PlayerPrefs.SetString(LastLoginKey, userId);
        PlayerPrefs.Save();

        loginFeedbackText.text = $"<color=green>{found.name}��, ȯ���մϴ�!</color>";

        // ���Ѵٸ� ���⼭ ���� ���� ������ ��ȯ
        // SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// ���� ���̵� �̹� �����ϴ��� ���θ� �˻��Ѵ�.
    /// </summary>
    /// <param name="userId">�˻��� ���̵�</param>
    /// <returns>�̹� �����ϸ� true, ������ false</returns>
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
    /// ���̵𡤺�й�ȣ�� ��ġ�ϴ� ȸ���� ������ �� UserData ��ü�� ��ȯ�ϰ�,  
    /// ������ null�� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="userId">�˻��� ���̵�</param>
    /// <param name="password">�˻��� ��й�ȣ</param>
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
    /// �ٸ� ������ UserDatabase ������Ʈ ��(��: �ܾ�/���� ���� ��), 
    /// AuthManager �ʿ����� �ٽ� �ҷ����� �ʹٸ� ȣ���� �� ����
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
