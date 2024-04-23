using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonNetWorkPool : IPunPrefabPool
{
    Dictionary<string, Queue<GameObject>> Spool;
    Dictionary<string, List<GameObject>> Susingpool;
    Dictionary<string, GameObject> prefabsCache;
    
    public PhotonNetWorkPool()
    {
        prefabsCache = new Dictionary<string, GameObject>();
        Spool = new Dictionary<string, Queue<GameObject>>();
        Susingpool = new Dictionary<string, List<GameObject>>();
    }

    public void AddResource(GameObject prefab)
    {
        prefabsCache.Add(prefab.name, prefab);
    }

    public void ReadyResource(string prefabId, int count)
    {
        GameObject prefab = null;

        if (prefabsCache.TryGetValue(prefabId, out prefab))
        {
            if (!Spool.ContainsKey(prefabId))
            {
                Spool.Add(prefabId, new Queue<GameObject>());
            }

            prefab.SetActive(false);

            for (int i = 0; i < count; i++)
            {
                GameObject instance = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                instance.name = prefab.name;
                Spool[prefabId].Enqueue(instance);
            }
        }
        else
        {
            Debug.LogError("AddResource로 prefab을 추가해주세요!");
            return;
        }
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;

        if (prefabsCache.TryGetValue(prefabId, out obj)) // 오브젝트 풀링을 사용하는 instantiate
        {
            if (Spool.ContainsKey(prefabId)) //프리팹이 false상태의 풀에 여유 풀이 존재할 경우!
            {
                if (!Susingpool.ContainsKey(prefabId))
                {
                    Susingpool.Add(prefabId, new List<GameObject>());
                }
                if (Spool[prefabId].Count > 0)
                {
                    obj = Spool[prefabId].Dequeue();
                }
                else
                {
                    ReadyResource(prefabId, 1);
                    obj = Spool[prefabId].Dequeue();
                }

                obj.transform.position = position;
                obj.transform.rotation = rotation;
                //obj.SetActive(true);
                Susingpool[prefabId].Add(obj);
                return obj;
            }
            else // 풀링을 사용하지 않는 instantiate
            {
                bool wasActive = obj.activeSelf;
                if (wasActive)
                {
                    obj.SetActive(false);
                }

                GameObject instance = Object.Instantiate(obj, position, rotation) as GameObject;
                if (wasActive)
                {
                    obj.SetActive(true);
                }

                return instance;
            }
        }
        else
        {
            Debug.LogError("AddResource로 prefab을 추가해주세요!");
            return null;
        }
    }

    public void Destroy(GameObject gameObject)
    {
        if (!Spool.ContainsKey(gameObject.name))
        {
            GameObject.Destroy(gameObject);
            return;
        }
        GameObject obj = null;
        if (Susingpool.ContainsKey(gameObject.name))
        {
            obj = Susingpool[gameObject.name].Find(d => d == gameObject);
            Susingpool[gameObject.name].Remove(obj);
        }
        obj.SetActive(false);
        Spool[gameObject.name].Enqueue(obj);
    }
}