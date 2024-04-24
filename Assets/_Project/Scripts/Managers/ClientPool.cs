using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Pool;

public class ClientPool
{
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary;
    private Transform parent;

    public ClientPool(Transform parent)
    {
        poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();
        this.parent = parent;
    }

    public GameObject Spawn(GameObject prefab, Vector2 position, Quaternion rotation)
    {
        string prefabName = prefab.name;
        
        if (!poolDictionary.ContainsKey(prefabName))
        {
            CreatePool(prefab, prefabName);
        }

        GameObject instance = poolDictionary[prefabName].Get();
        
        instance.transform.parent = parent;
        instance.transform.position = position;
        instance.transform.rotation = rotation;

        return instance;
    }

    public GameObject Spawn(GameObject prefab, Transform parent = null)
    {
        string prefabName = prefab.name;

        if (!poolDictionary.ContainsKey(prefabName))
        {
            CreatePool(prefab, prefabName);
        }

        GameObject instance = poolDictionary[prefabName].Get();

        if (parent == null)
        {
            instance.transform.parent = this.parent;
        }
        else
        {
            instance.transform.parent = parent;
        }

        instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        return instance;
    }

    private void CreatePool(GameObject prefab, string prefabName)
    {
        ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = UnityEngine.Object.Instantiate(prefab);
                obj.gameObject.name = prefabName;

                return obj;
            },
            actionOnGet: (GameObject obj) =>
            {
                obj.gameObject.SetActive(true);
            },
            actionOnRelease: (GameObject obj) =>
            {
                obj.gameObject.SetActive(false);
            },
            actionOnDestroy: (GameObject obj) =>
            {
                UnityEngine.Object.Destroy(obj);
            }
            );
        poolDictionary.Add(prefabName, objectPool);
    }

    public void Despawn(GameObject gameObject)
    {
        string objectName = gameObject.gameObject.name;
        if (poolDictionary.ContainsKey(objectName))
        {
            poolDictionary[objectName].Release(gameObject);
        }
    }
}