using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviourPun
{
    public static PoolManager Instance { get; private set; }
    public PhotonNetworkPool networkPool;
    public ClientPool clientPool;

    public string path;
    public int count;

    private void Awake()
    {
        Instance = this;
        networkPool = new PhotonNetworkPool();
        clientPool = new ClientPool(transform);
    }
}
