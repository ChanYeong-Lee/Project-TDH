using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviourPun
{
    [PunRPC]
    public void SetActiveRPC(bool active)
    {
        gameObject.SetActive(active);
    }
}
