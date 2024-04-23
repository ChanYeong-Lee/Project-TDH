using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    public PhotonNetWorkPool photonPool;
    public DefaultPool defaultPool;
    public ClientPool clientPool;

    private void Awake()
    {
        Instance = this;
    }

    public void InitPhotonPool()
    {
        photonPool = new PhotonNetWorkPool();
        PhotonNetwork.PrefabPool = photonPool;
    }
}
