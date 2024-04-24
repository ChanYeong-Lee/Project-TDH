using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviourPun, INetworkPool
{
    public EnemyBrain brain;
    public EnemyMove move;
    public EnemyHealth health;

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

    [PunRPC]
    public void SetActiveRPC(bool active)
    {
        gameObject.SetActive(active);
    }
}
