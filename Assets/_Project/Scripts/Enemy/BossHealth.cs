using UnityEngine;
using UnityEngine.UI;

public class BossHealth : EnemyHealth
{
    public Image bossHealthBar;

    protected override void OnEnable()
    {
        base.OnEnable();
        bossHealthBar = UIManager.Instance.bossHealthBarFillImage;
        UIManager.Instance.bossHealthBar.gameObject.SetActive(true);
    }

    protected override void Update()
    {
        base.Update();
        if (bossHealthBar != null)
        {
            bossHealthBar.fillAmount = currentHP / Mathf.Clamp(maxHP, 0.1f, maxHP);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIManager.Instance.bossHealthBar.gameObject.SetActive(false);
    }
}
