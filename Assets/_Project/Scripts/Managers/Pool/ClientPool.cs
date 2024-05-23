using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using UnityEditor;
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

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string prefabName = prefab.name;
        
        if (poolDictionary.ContainsKey(prefabName) == false)
        {
            CreatePool(prefab, prefabName);
        }

        GameObject instance = poolDictionary[prefabName].Get();
        
        //instance.transform.parent = parent;
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.SetActive(true);

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
            instance.transform.SetParent(this.parent);
        }
        else
        {
            instance.transform.SetParent(parent);
        }

        instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        instance.SetActive(true);

        return instance;
    }

    private void CreatePool(GameObject prefab, string prefabName)
    {
        ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = UnityEngine.Object.Instantiate(prefab);
                obj.gameObject.name = prefabName;
                obj.gameObject.SetActive(false);

                return obj;
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
        if (gameObject.activeSelf == false)
        {
            return;
        }

        string objectName = gameObject.name;
        if (poolDictionary.ContainsKey(objectName))
        {
            poolDictionary[objectName].Release(gameObject);
        }
        else
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}