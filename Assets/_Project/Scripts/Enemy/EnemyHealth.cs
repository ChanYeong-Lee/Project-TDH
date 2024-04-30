using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviourPun
{
    public float currentHP;
    public float maxHP;
    
    public float defense;
    public float defenseIncrease;

    public float hpRecovery;
    public float hpRecoveryIncrease;

    public Image healthBar;

    protected virtual void OnEnable()
    {
        currentHP = maxHP;
    }

    protected virtual void OnDisable()
    {
        
    }


    protected virtual void Update()
    {
        if (currentHP < maxHP)
        {
            float recovery = hpRecovery * hpRecoveryIncrease;
            currentHP += recovery * Time.deltaTime;
        }

        currentHP = Mathf.Clamp(currentHP, 0.0f, maxHP);

        if (healthBar != null)
        {
            healthBar.fillAmount = currentHP / Mathf.Clamp(maxHP, 0.1f, maxHP);
        }
    }

    public void TakeHit(float normalDamage, float trueDamage)
    {
        photonView.RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);   
    }

    [PunRPC]
    protected virtual void TakeHitRPC(float normalDamage, float trueDamage, PhotonMessageInfo info)
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }
        float defenseAmount = defense * defenseIncrease;
        float damage = normalDamage - defenseAmount + trueDamage;
        
        currentHP -= damage;

        print($"데미지를 {damage}만큼 입었습니다.");

        if (currentHP <= 0)
        {
            currentHP = 0.0f;
            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.networkPool.Despawn(gameObject);
            }
        }
    }

    [PunRPC]
    private void TestFunc(int number)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == number)
        {
            // DO Somthing;
        }
    }
}
