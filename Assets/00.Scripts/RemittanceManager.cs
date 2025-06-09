using UnityEngine;
using TMPro;

public class RemittanceManager : MonoBehaviour
{
    [Header("송금 UI References")]
    [Tooltip("수신자 ID 또는 이름을 입력하는 InputField")]
    public TMP_InputField recipientInputField;

    [Tooltip("송금할 금액을 입력하는 InputField (숫자만)")]
    public TMP_InputField amountInputField;

    [Tooltip("오류 또는 성공 메시지를 표시할 Text")]
    public TMP_Text errorText;

    [Tooltip("송금 완료 후, 성공 메시지를 표시할 Text (선택)")]
    public TMP_Text successText;

    private void Start()
    {
        // 초기에는 오류/성공 메시지를 비웁니다.
        if (errorText != null)
            errorText.text = "";

        if (successText != null)
            successText.text = "";
    }

    // TransferButton (송금 버튼) 의 OnClick 이벤트에 연결될 메서드
    public void OnClick_Transfer()
    {
        // 입력 필드에서 수신자와 금액을 가져오기
        string recipientIdOrName = recipientInputField.text.Trim();
        string amountStr = amountInputField.text.Trim();

        // 오류 메시지 초기화
        if (errorText != null)
            errorText.text = "";
        if (successText != null)
            successText.text = "";

        // 수신자 미입력 검사
        if (string.IsNullOrEmpty(recipientIdOrName))
        {
            if (errorText != null)
                errorText.text = "<color=red>송금 대상(ID 또는 이름)을 입력해주세요.</color>";
            return;
        }

        // 금액 미입력 또는 숫자 변환 실패 검사
        if (string.IsNullOrEmpty(amountStr))
        {
            if (errorText != null)
                errorText.text = "<color=red>송금할 금액을 입력해주세요.</color>";
            return;
        }

        int amount;
        bool isNumber = int.TryParse(amountStr, out amount);
        if (!isNumber || amount <= 0)
        {
            if (errorText != null)
                errorText.text = "<color=red>송금할 금액은 1 이상의 숫자로 입력해주세요.</color>";
            return;
        }

        // GameManager 싱글톤 존재 여부 체크
        if (GameManager.Instance == null)
        {
            if (errorText != null)
                errorText.text = "<color=red>GameManager를 찾을 수 없습니다.</color>";
            return;
        }

        // 실제 송금 로직 수행
        string transferError;
        bool success = GameManager.Instance.TransferTo(recipientIdOrName, amount, out transferError);

        if (!success)
        {
            // 송금 중 오류 발생
            if (errorText != null)
                errorText.text = $"<color=red>{transferError}</color>";
        }
        else
        {
            // 송금 성공
            if (successText != null)
                successText.text = $"<color=green>송금이 완료되었습니다. ({recipientIdOrName}님에게 {amount:N0}원)</color>";

            // 송금 후에는 입력 필드 비우기
            recipientInputField.text = "";
            amountInputField.text = "";

            // 오류 메시지도 혹시 표시되어 있다면 지우기
            if (errorText != null)
                errorText.text = "";
        }
    }
}
