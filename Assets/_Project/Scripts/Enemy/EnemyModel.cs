using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviourPun, INetworkPool, IModel
{
    public EnemyBrain brain;
    public EnemyMove move;
    public EnemyHealth health;
    public Dictionary<string, Buff> buffDictionary;
    public Action<EnemyModel> onDisable;

    public int poolCount;

    public float syncTimeoutDelta = 0.0f;

    private void Awake()
    {
        brain = GetComponent<EnemyBrain>();
        move = GetComponent<EnemyMove>();
        health = GetComponent<EnemyHealth>();
        buffDictionary = new Dictionary<string, Buff>();
    }

    private void OnEnable()
    {
        EnemyManager.Instance.enemies.Add(this);
        poolCount++;
    }

    private void OnDisable()
    {
        EnemyManager.Instance.enemies.Remove(this);
        foreach (Buff buff in buffDictionary.Values)
        {
            if (buff.buffCoroutine != null)
            {
                StopCoroutine(buff.buffCoroutine);
            }
            buff.Deactivate();
        }
        buffDictionary.Clear();

        onDisable?.Invoke(this);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (syncTimeoutDelta <= 0.0f)
            {
                photonView.RPC("SyncPosition", RpcTarget.Others, transform.position, move.applyMoveSpeed);
                syncTimeoutDelta = 1.0f;
            }
            else
            {
                syncTimeoutDelta -= Time.deltaTime;
            }
        }
    }

   
    [PunRPC]
    public void SetActiveRPC(bool active, Vector3 position, Quaternion roation)
    {
        if (active)
        {
            transform.position = position;
            transform.rotation = roation;
        }

        gameObject.SetActive(active);
    }

    [PunRPC]
    public void SyncPosition(Vector3 position, float moveSpeed, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        transform.position = position + lag * moveSpeed * transform.forward;
    }

    [PunRPC]
    public void AddBuff
        (int casterId, int modelPoolCount, 
        string buffName, BuffType buffType, StatType statType, 
        float increaseAmount, float limitTime, PhotonMessageInfo info)
    {
        if (gameObject.activeSelf == false
            || modelPoolCount != poolCount)
        {
            return;
        }
        
        CharacterModel caster = CharacterManager.Instance.wholeCharacters.Find((model) => model.photonView.ViewID == casterId);

        if (caster == null)
        {
            return;
        }

        if (buffDictionary.ContainsKey(buffName))
        {
            if (buffDictionary[buffName].buffType == BuffType.Limit)
            {
                StopCoroutine(buffDictionary[buffName].buffCoroutine);
            }
            buffDictionary[buffName].Deactivate();
        }

        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        Buff newBuff = new Buff();
        
        newBuff.SetBuff(caster, this, buffName, buffType, statType, increaseAmount, limitTime - lag);
        buffDictionary[buffName] = newBuff;

        switch (buffType)
        {
            case BuffType.Permanant:
                newBuff.Activate();
                break;
            case BuffType.Limit:
                newBuff.buffCoroutine = StartCoroutine(newBuff.BuffCoroutine());
                break;
        }
    }

    [PunRPC]
    public void RemoveBuff(string buffName)
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }

        if (buffDictionary.ContainsKey(buffName) == false)
        {
            return;
        }

        buffDictionary[buffName].Deactivate();
        buffDictionary.Remove(buffName);
    }
}
