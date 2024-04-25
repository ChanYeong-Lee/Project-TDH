using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : EnemyHealth
{
    protected override void OnEnable()
    {
        base.OnEnable();
        healthBar = UIManager.Instance.bossHealthBarFillImage;
        UIManager.Instance.bossHealthBar.gameObject.SetActive(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIManager.Instance.bossHealthBar.gameObject.SetActive(false);
    }
}
