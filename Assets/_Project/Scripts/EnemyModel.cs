using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviourPun, INetworkPool
{
    public EnemyBrain brain;
    public EnemyMove move;
    public EnemyHealth health;

    public float syncTimeoutDelta = 0.0f;


    private void Awake()
    {
        brain = GetComponent<EnemyBrain>();
        move = GetComponent<EnemyMove>();
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        EnemyManager.Instance.enemies.Add(this);
    }

    private void OnDisable()
    {
        EnemyManager.Instance.enemies.Remove(this);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (syncTimeoutDelta <= 0.0f)
            {
                photonView.RPC("SyncPosition", RpcTarget.Others, transform.position, move.moveSpeed * move.moveIncrease);
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
        transform.position = position + lag * moveSpeed * move.velocity;
    }
}
