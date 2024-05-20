using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPun
{
    public static NetworkManager Instance { get; private set; }

    public void Init()
    {
        Instance = this;
    }

    [PunRPC]
    public void GiveRandomCrystal()
    {
        GameManager.Instance.defensePlayer.AddRandomCrystal();
    }

    [PunRPC]
    public void GiveSpecialCrystal()
    {
        GameManager.Instance.defensePlayer.AddSpecialCrystal();
    }
}