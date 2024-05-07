using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviourPun, INetworkPool, IModel
{
    public EnemyBrain brain;
    public EnemyMove move;
    public EnemyHealth health;
    public Dictionary<string, Buff> buffDictionary;

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
    }

    private void OnDisable()
    {
        EnemyManager.Instance.enemies.Remove(this);
        buffDictionary.Clear();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (syncTimeoutDelta <= 0.0f)
            {
                photonView.RPC("SyncPosition", RpcTarget.Others, transform.position, move.moveSpeed * move.moveSpeedIncrease);
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
    public void AddBuff(int casterId, string buffName, BuffType buffType, StatType statType, float increaseAmount, float limitTime)
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }

        if (buffDictionary.ContainsKey(buffName))
        {
            StopCoroutine(buffDictionary[buffName].buffCoroutine);
            buffDictionary[buffName].Deactivate();
        }

        Buff newBuff = new Buff();
        CharacterModel caster = CharacterManager.Instance.wholeCharacters.Find((model) => model.photonView.ViewID == casterId);

        if (caster == null)
        {
            return;
        }

        newBuff.SetBuff(caster, this, buffName, buffType, statType, increaseAmount, limitTime);
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
    }
}
