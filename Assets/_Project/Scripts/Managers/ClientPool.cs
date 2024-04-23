using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ClientPool : MonoBehaviour
{
    Dictionary<string, ObjectPool<GameObject>> pool = new Dictionary<string, ObjectPool<GameObject>>();
    //DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
    //if (pool != null && objects != null)
    //{
    //    foreach (GameObject prefab in objects)
    //    {
    //        pool.ResourceCache.Add(prefab.name, prefab);

    //    }
    //}

    ////어떤시점에 원래 PhotonNetwork.Instantiate(""test1"");

    //pool.Instantiate(""Enemy"", Vector3.zero, Quaternion.identity);
    public GameObject Get(GameObject g, Vector2 pos, Quaternion rot, Transform t = null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g, str);
        }
        GameObject obj = pool[str].Get();
        if (t == null)
            obj.transform.parent = transform;
        else
            obj.transform.parent = t;
        obj.transform.position = pos;
        return obj;

        #region 안씀
        //GameObject obj = pool[str].Dequeue();
        //if (usingpool == null) usingpool = new Dictionary<string, List<GameObject>>();
        //if (!usingpool.ContainsKey(str))
        //{
        //    usingpool.Add(str, new List<GameObject>());
        //}
        //usingpool[str].Add(obj);
        //obj.gameObject.SetActive(true);
        //obj.transform.position = pos;
        //obj.transform.rotation = rot;
        //callback?.Invoke();
        //return obj;
        #endregion
    }
    public GameObject Get(GameObject g, Transform t = null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g, str);
        }
        GameObject obj = pool[str].Get();
        if (t == null)
            obj.transform.parent = transform;
        else
            obj.transform.parent = t;
        return obj;

    }
    public GameObject Get<T>(GameObject g, out T cls, Transform t = null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g, str);
        }
        GameObject obj = pool[str].Get();
        if (t == null)
            obj.transform.parent = transform;
        else
        {
            obj.transform.parent = t;
        }
        obj.TryGetComponent<T>(out cls);
        return obj;

    }
    public GameObject Get<T>(GameObject g, Vector2 pos, Quaternion rot, out T cls, Transform t = null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g, str);
        }
        GameObject obj = pool[str].Get();

        if (t == null)
            obj.transform.parent = transform;
        else
            obj.transform.parent = t;
        obj.transform.position = pos;
        obj.TryGetComponent<T>(out cls);
        return obj;


    }
    private void CreatePool(GameObject prefab, string key)
    {
        ObjectPool<GameObject> pool_ = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab);
                obj.gameObject.name = key;

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
                Destroy(obj);
            }
            );
        pool.Add(key, pool_);
    }

    public void Despawn(GameObject g)
    {

        String str = g.gameObject.name;
        if (pool.ContainsKey(str))
        {
            pool[str].Release(g);
        }
        #region 안씀
        //if (!pool.ContainsKey(str))
        //{

        //    pool.Add(str, new Queue<GameObject>());
        //}



        //if (usingpool.ContainsKey(str))
        //{
        //    obj = usingpool[str].Find(d => d == g);
        //}
        //if (obj == null)
        //{
        //    if (tr != null)
        //    {
        //        obj = GameObject.Instantiate(g, tr);

        //    }
        //    else
        //    {
        //        obj = GameObject.Instantiate(g);
        //    }
        //    obj.name = str;
        //}
        //else
        //{
        //    usingpool[str].Remove(obj);
        //}
        //obj.SetActive(false);
        //pool[str].Enqueue(obj);
        #endregion
    }
}