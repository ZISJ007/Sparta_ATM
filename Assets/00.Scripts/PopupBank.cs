using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupBank : MonoBehaviour
{
    [Header("Bank Panels")]
    public GameObject atmPanel;
    public GameObject depositPanel;
    public GameObject withdrawalPanel;
    public GameObject notEnoughMoneyUI;
    [Header("Input Fields")]
    public TMP_InputField depositInputField; // 입금 입력 필드
    public TMP_InputField withdrawInputField; // 출금 입력 필드

    private bool isDepositMode = false; // 입금 모드 여부
    private bool isWithdrawalMode = false; // 출금 모드 여부
    private void Start()
    {
        atmPanel.SetActive(true); // ATM 패널을 기본으로 활성화
        depositPanel.SetActive(false); // 입금 패널 비활성화
        withdrawalPanel.SetActive(false); // 출금 패널 비활성화
        notEnoughMoneyUI.SetActive(false); // 현금 부족 UI 비활성화
    }
    public void OnInputBank(int amount)
    {
        if (isDepositMode)
        {
            if (int.TryParse(depositInputField.text, out amount))
            {
                if (GameManager.Instance.userData.cash >= amount)
                {
                    GameManager.Instance.Deposit(amount);
                }
                else
                {
                    notEnoughMoneyUI.SetActive(true);
                }
            }
        }
        else if (isWithdrawalMode)
        {
            if (int.TryParse(withdrawInputField.text, out amount))
            {
                if (GameManager.Instance.userData.balance >= amount)
                {
                    GameManager.Instance.Withdraw(amount);
                }
                else
                {
                    notEnoughMoneyUI.SetActive(true);
                }
            }
        }
    }
  
    public void OnClickBank(int amount)
    {
        if(isWithdrawalMode)
        {
            if (GameManager.Instance.userData.balance >= amount && GameManager.Instance.userData.balance >= 0) //잔액이 충분한지 확인
            {
                GameManager.Instance.Withdraw(amount);
            }
            else
            {
                NotEnough();
            }
        } 
        else if(isDepositMode)
        {
            if (GameManager.Instance.userData.cash >= amount && GameManager.Instance.userData.cash >= 0) //현금이 충분한지 확인
            {
                GameManager.Instance.Deposit(amount);
            }
            else
            {
                NotEnough();
            }
        }

    }
    private void NotEnough()
    {
        notEnoughMoneyUI.SetActive(true); // 잔액 또는 현금이 부족할 경우 UI 활성화
    }
    public void OpenDeposit()
    {
        atmPanel.SetActive(false);
        depositPanel.SetActive(true);
        isDepositMode = true; // 입금 모드 활성화
    }
    public void OpenWithdrawal()
    {
        atmPanel.SetActive(false);
        withdrawalPanel.SetActive(true);
        isWithdrawalMode = true; // 출금 모드 활성화
    }
    public void CloseBank()
    {
        withdrawalPanel.SetActive(false);
        depositPanel.SetActive(false);
        atmPanel.SetActive(true);
        isDepositMode = false; // 입금 모드 비활성화
        isWithdrawalMode = false; // 출금 모드 비활성화
    }
    public void ClosePopUp()
    {
        notEnoughMoneyUI.SetActive(false); // 현금 부족 UI 닫기
    }
}
