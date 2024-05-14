using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviourPun
{
    public float hpAmount => currentHP / Mathf.Clamp(maxHP, 0.1f, maxHP);
    public float currentHP;
    public float maxHP;

    public float applyDefense => defense + defenseIncrease;
    public float defense;
    public float defenseIncrease;

    public float hpRecovery;
    public float hpRecoveryIncrease;

    public Image healthBar;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>(true);
    }

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
            canvas.enabled = hpAmount < 1.0f;
            healthBar.fillAmount = hpAmount;
        }
    }

    public void TakeHit(float normalDamage, float trueDamage)
    {
        photonView.RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);   
    }

    [PunRPC]
    protected virtual void TakeHitRPC(int poolCount, float normalDamage, float trueDamage, PhotonMessageInfo info)
    {
        int modelPoolCount = GetComponent<EnemyModel>().poolCount;

        if (gameObject.activeSelf == false
            || poolCount != modelPoolCount)
        {
            return;
        }

        float applyNormalDamage = Mathf.Clamp((normalDamage - applyDefense), 0.0f, Mathf.Infinity);
        float damage = Mathf.Clamp(applyNormalDamage + trueDamage, 0.0f, Mathf.Infinity);

        currentHP -= damage;

        print($"데미지를 normal = {applyNormalDamage}, true = {trueDamage} => damage = {damage} 만큼 입었습니다.");

        if (currentHP <= 0)
        {
            currentHP = 0.0f;
            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.networkPool.Despawn(gameObject);
            }
        }
        
        
    }
}
