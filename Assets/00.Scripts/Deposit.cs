using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit : MonoBehaviour
{
    public int depositAmount = 0; // Amount to deposit
    public void OnClickDeposit()
    { 
        GameManager.Instance.userData.balance += depositAmount; //����ݾ׿� �Աݾ� �߰�
        GameManager.Instance.userData.cash -= depositAmount; //���ݿ��� �Աݾ� ����
    }
    public void BackDeposit()
    {

    }
}
