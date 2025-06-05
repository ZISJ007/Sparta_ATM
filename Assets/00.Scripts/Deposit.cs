using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit : MonoBehaviour
{
    public int depositAmount = 0; // Amount to deposit
    public void OnClickDeposit()
    { 
        GameManager.Instance.userData.balance += depositAmount; //통장금액에 입금액 추가
        GameManager.Instance.userData.cash -= depositAmount; //현금에서 입금액 차감
    }
    public void BackDeposit()
    {

    }
}
