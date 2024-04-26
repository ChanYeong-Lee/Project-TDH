using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    private CharacterModel model;
    private Skill coolDownSkill;

    public RectTransform Indicator;
    public RectTransform mainSelected;

    public RectTransform coolDown;
    public Image coolDownFillImage;

    public RectTransform attackRange;

    private void Awake()
    {
        model = GetComponentInParent<CharacterModel>();
        coolDownSkill = model.skill.skills.Find((skill) => skill.type == SkillType.CoolDown);
    }

    private void Update()
    {
        if (model == null)
        {
            return;
        }

        attackRange.localScale = model.attack.attackRange * model.attack.attackRangeIncrease * Vector3.one;

        if (coolDownSkill != null)
        {
            coolDownFillImage.fillAmount = coolDownSkill.coolDownAmount;
        }
    }
}
