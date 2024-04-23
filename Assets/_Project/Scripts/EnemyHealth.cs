using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviourPun
{
    public float currentHP;
    public float maxHP;
    
    public float defense;
    public float defenseIncrease;

    public float hpRecovery;
    public float hpRecoveryIncrease;

    private void Update()
    {
        if (currentHP < maxHP)
        {
            float recovery = hpRecovery * hpRecoveryIncrease;
            currentHP += recovery * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0.0f, maxHP);
        }
    }

    public void TakeHit(float normalDamage, float trueDamage)
    {
        photonView.RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
    }

    [PunRPC]
    private void TakeHitRPC(float normalDamage, float trueDamage)
    {
        float defenseAmount = defense * defenseIncrease;
        float damage = normalDamage - defenseAmount + trueDamage;
        currentHP -= damage;
        print("TakeHit!");
        if (currentHP <= 0)
        {
            currentHP = 0.0f;
            // Á×´Â´Ù.
        }
    }
}
