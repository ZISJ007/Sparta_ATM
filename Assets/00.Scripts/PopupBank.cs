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
    public TMP_InputField depositInputField; // �Ա� �Է� �ʵ�
    public TMP_InputField withdrawInputField; // ��� �Է� �ʵ�

    private bool isDepositMode = false; // �Ա� ��� ����
    private bool isWithdrawalMode = false; // ��� ��� ����
    private void Start()
    {
        atmPanel.SetActive(true); // ATM �г��� �⺻���� Ȱ��ȭ
        depositPanel.SetActive(false); // �Ա� �г� ��Ȱ��ȭ
        withdrawalPanel.SetActive(false); // ��� �г� ��Ȱ��ȭ
        notEnoughMoneyUI.SetActive(false); // ���� ���� UI ��Ȱ��ȭ
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
            if (GameManager.Instance.userData.balance >= amount && GameManager.Instance.userData.balance >= 0) //�ܾ��� ������� Ȯ��
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
            if (GameManager.Instance.userData.cash >= amount && GameManager.Instance.userData.cash >= 0) //������ ������� Ȯ��
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
        notEnoughMoneyUI.SetActive(true); // �ܾ� �Ǵ� ������ ������ ��� UI Ȱ��ȭ
    }
    public void OpenDeposit()
    {
        atmPanel.SetActive(false);
        depositPanel.SetActive(true);
        isDepositMode = true; // �Ա� ��� Ȱ��ȭ
    }
    public void OpenWithdrawal()
    {
        atmPanel.SetActive(false);
        withdrawalPanel.SetActive(true);
        isWithdrawalMode = true; // ��� ��� Ȱ��ȭ
    }
    public void CloseBank()
    {
        withdrawalPanel.SetActive(false);
        depositPanel.SetActive(false);
        atmPanel.SetActive(true);
        isDepositMode = false; // �Ա� ��� ��Ȱ��ȭ
        isWithdrawalMode = false; // ��� ��� ��Ȱ��ȭ
    }
    public void ClosePopUp()
    {
        notEnoughMoneyUI.SetActive(false); // ���� ���� UI �ݱ�
    }
}
