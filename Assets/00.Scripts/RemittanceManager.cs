using UnityEngine;
using TMPro;

public class RemittanceManager : MonoBehaviour
{
    [Header("�۱� UI References")]
    [Tooltip("������ ID �Ǵ� �̸��� �Է��ϴ� InputField")]
    public TMP_InputField recipientInputField;

    [Tooltip("�۱��� �ݾ��� �Է��ϴ� InputField (���ڸ�)")]
    public TMP_InputField amountInputField;

    [Tooltip("���� �Ǵ� ���� �޽����� ǥ���� Text")]
    public TMP_Text errorText;

    [Tooltip("�۱� �Ϸ� ��, ���� �޽����� ǥ���� Text (����)")]
    public TMP_Text successText;

    private void Start()
    {
        // �ʱ⿡�� ����/���� �޽����� ���ϴ�.
        if (errorText != null)
            errorText.text = "";

        if (successText != null)
            successText.text = "";
    }

    // TransferButton (�۱� ��ư) �� OnClick �̺�Ʈ�� ����� �޼���
    public void OnClick_Transfer()
    {
        // �Է� �ʵ忡�� �����ڿ� �ݾ��� ��������
        string recipientIdOrName = recipientInputField.text.Trim();
        string amountStr = amountInputField.text.Trim();

        // ���� �޽��� �ʱ�ȭ
        if (errorText != null)
            errorText.text = "";
        if (successText != null)
            successText.text = "";

        // ������ ���Է� �˻�
        if (string.IsNullOrEmpty(recipientIdOrName))
        {
            if (errorText != null)
                errorText.text = "<color=red>�۱� ���(ID �Ǵ� �̸�)�� �Է����ּ���.</color>";
            return;
        }

        // �ݾ� ���Է� �Ǵ� ���� ��ȯ ���� �˻�
        if (string.IsNullOrEmpty(amountStr))
        {
            if (errorText != null)
                errorText.text = "<color=red>�۱��� �ݾ��� �Է����ּ���.</color>";
            return;
        }

        int amount;
        bool isNumber = int.TryParse(amountStr, out amount);
        if (!isNumber || amount <= 0)
        {
            if (errorText != null)
                errorText.text = "<color=red>�۱��� �ݾ��� 1 �̻��� ���ڷ� �Է����ּ���.</color>";
            return;
        }

        // GameManager �̱��� ���� ���� üũ
        if (GameManager.Instance == null)
        {
            if (errorText != null)
                errorText.text = "<color=red>GameManager�� ã�� �� �����ϴ�.</color>";
            return;
        }

        // ���� �۱� ���� ����
        string transferError;
        bool success = GameManager.Instance.TransferTo(recipientIdOrName, amount, out transferError);

        if (!success)
        {
            // �۱� �� ���� �߻�
            if (errorText != null)
                errorText.text = $"<color=red>{transferError}</color>";
        }
        else
        {
            // �۱� ����
            if (successText != null)
                successText.text = $"<color=green>�۱��� �Ϸ�Ǿ����ϴ�. ({recipientIdOrName}�Կ��� {amount:N0}��)</color>";

            // �۱� �Ŀ��� �Է� �ʵ� ����
            recipientInputField.text = "";
            amountInputField.text = "";

            // ���� �޽����� Ȥ�� ǥ�õǾ� �ִٸ� �����
            if (errorText != null)
                errorText.text = "";
        }
    }
}
