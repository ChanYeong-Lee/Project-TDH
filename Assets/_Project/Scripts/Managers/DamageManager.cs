using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set; }
    
    public List<DamageInfo> damageList;
    private WaitForSeconds oneSecond;

    private void Awake()
    {
        Instance = this;
        damageList = new List<DamageInfo>();    
    }

    private void Start()
    {
        oneSecond = new WaitForSeconds(1.0f);
        StartCoroutine(CalulateDPSCoroutine());
    }

    public void AddDamage(int viewID, float damage)
    {
        DamageInfo info = damageList.Find(element => element.viewID == viewID);
        if (info == null)
        {
            info = new DamageInfo() { viewID = viewID, damage = 0.0f };
            damageList.Add(info);
        }

        info.damage += damage;
        info.damageAtSeconds += damage;
    }

    public DamageInfo GetDamageInfo(int viewID)
    {
        return damageList.Find(element => element.viewID == viewID);
    }

    private IEnumerator CalulateDPSCoroutine()
    {
        while (true)
        {
            foreach (DamageInfo info in damageList)
            {
                info.damagePerSeconds = info.damageAtSeconds;
                info.damageAtSeconds = 0.0f;
            }

            damageList = damageList.OrderByDescending(element => element.damagePerSeconds).ToList();
            UIManager.Instance.damage.UpdateDamageUI();

            yield return oneSecond;
        }
    }
}

public class DamageInfo
{
    public int viewID;
    public float damage;
    public float damageAtSeconds;
    public float damagePerSeconds;
}