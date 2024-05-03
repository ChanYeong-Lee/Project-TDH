using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    private CharacterModel model;
    private Skill coolDownSkill;

    public RectTransform indicator;
    public RectTransform mainSelected;

    public RectTransform coolDown;
    public Image coolDownFillImage;

    public RectTransform attackRange;

    private void Awake()
    {
        model = GetComponentInParent<CharacterModel>();
    }

    private void Start()
    {
        coolDownSkill = model.skill.skills.Find((skill) => skill.defaultStat.skillType == SkillType.NonTargetCooldown);
        if (coolDownSkill == null)
        {
            coolDownSkill = model.skill.skills.Find((skill) => skill.defaultStat.skillType == SkillType.TargetCooldown);
        }

        if (coolDownSkill == null)
        {
            coolDown.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (model == null)
        {
            return;
        }

        if (attackRange.gameObject.activeSelf)
        {
            attackRange.localScale = model.attack.attackRange * model.attack.attackRangeIncrease * Vector3.one;
        }

        if (coolDownSkill != null)
        {
            coolDown.gameObject.SetActive(true);
            coolDownFillImage.fillAmount = coolDownSkill.coolDownAmount;
        }
    }

    public void Select(bool main, bool showAttackRange = false)
    {
        indicator.gameObject.SetActive(true); 
        mainSelected.gameObject.SetActive(main);
        attackRange.gameObject.SetActive(main && showAttackRange);
    }

    public void Deselect()
    {
        indicator.gameObject.SetActive(false);
        mainSelected.gameObject.SetActive(false);
        attackRange.gameObject.SetActive(false);
    }
}

