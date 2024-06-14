using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface INetworkPool
{
    void SetActiveRPC(bool active, Vector3 position, Quaternion rotation);
}

public class PhotonNetworkPool
{
    private Dictionary<string, List<GameObject>> poolDictionary;

    public PhotonNetworkPool()  
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();
    }

    public void PreSpawn(string prefabId, int count, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.ContainsKey(prefabId) == false)
        {
            poolDictionary.Add(prefabId, new List<GameObject>());
        }

        for (int i = 0; i < count; i++)
        {
            GameObject instance = PhotonNetwork.Instantiate(prefabId, position, rotation);
            instance.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.AllBuffered, false, position, rotation);
            poolDictionary[prefabId].Add(instance);
        }
    }

    public GameObject Spawn(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.ContainsKey(prefabId) == false)
        {
            poolDictionary.Add(prefabId, new List<GameObject>());
        }

        GameObject instance = poolDictionary[prefabId].Find(element => element.activeSelf == false);

        if (instance == null)
        {
            instance = PhotonNetwork.Instantiate(prefabId, position, rotation);
            poolDictionary[prefabId].Add(instance);
        }
        else
        {
            instance.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.AllBuffered, true, position, rotation);
        }

        return instance;
    }
    
    public void Despawn(GameObject despawnObject)
    {
        despawnObject.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.AllBuffered, false, Vector3.zero, Quaternion.identity);
    }
}