using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set; }
    
    public List<DamageInfo> damageList;

    private void Awake()
    {
        Instance = this;
    }

    public void AddDamage(int viewID, float damage)
    {
        if (damageList.Exists(element => element.viewID == viewID) == false)
        {
            DamageInfo newInfo = new DamageInfo() { viewID = viewID, damage = 0.0f };   
            damageList.Add(newInfo);    
        }

        DamageInfo info = damageList.Find(element=>element.viewID == viewID);
        info.damage += damage;

        damageList = damageList.OrderBy(element => element.damage).ToList();

        UIManager.Instance.damage.UpdateDamageUI();
    }

    public float GetDamage(int viewID)
    {
        if (damageList.Exists(element => element.viewID == viewID))
        {
            return damageList.Find(element => element.viewID == viewID).damage;
        }
        else
        {
            return 0.0f;
        }
    }
}

public struct DamageInfo
{
    public int viewID;
    public float damage;
}