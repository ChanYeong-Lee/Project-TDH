using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInformationUI : MonoBehaviour
{
    [Header("Title")]
    public Image skillIcon;
    public TMP_Text skillNameText;
    public TMP_Text targetTypeText;
    public TMP_Text skillTypeText;

    [Header("Description")]
    public TMP_Text descriptionText;

    [Header("Amount")]
    public RectTransform skillPercentRect;
    public TMP_Text skillPercentText;
    public RectTransform cooldownRect;
    public TMP_Text cooldownText;

    [Header("상태")]
    public Skill skill;

    //private void Update()
    //{
    //    if (skill == null)
    //    {
    //        return;
    //    }

    //    switch (skill.defaultStat.skillType)
    //    {
    //        case SkillType.NonTargetCooldown:
    //        case SkillType.TargetCooldown:
    //            cooldownText.text = $"{skill.applyCooldown:F2}초";
    //            break;
    //        case SkillType.Random:
    //            skillPercentText.text = $"{skill.applyPercentage:P1}";
    //            break;
    //    }
    //}

    public void SetSkill(Skill skill)
    {
        this.skill = skill;

        if (skill == null)
        {
            return;
        }

        skillIcon.sprite = skill.defaultStat.skillIcon;
        skillNameText.text = skill.defaultStat.skillName;
        descriptionText.text = skill.defaultStat.description;

        switch (skill.defaultStat.mainTargetType)
        {
            case TargetType.Enemy:
                targetTypeText.text = "적군";
                break;
            case TargetType.AllAlly:
                targetTypeText.text = "아군";
                break;
            case TargetType.StrongestAlly:
                targetTypeText.text = "아군";
                break;
            case TargetType.Self:
                targetTypeText.text = "자신";
                break;
        }

        switch (skill.defaultStat.skillType)
        {
            case SkillType.Always:
                skillTypeText.text = "항상";
                skillPercentRect.gameObject.SetActive(false);
                cooldownRect.gameObject.SetActive(false);
                break;
            case SkillType.NonTargetCooldown:
                skillTypeText.text = "쿨다운";
                skillPercentRect.gameObject.SetActive(false);
                cooldownRect.gameObject.SetActive(true);
                cooldownText.text = $"{skill.applyCooldown:F2}초";
                break;
            case SkillType.TargetCooldown:
                skillTypeText.text = "쿨다운";
                skillPercentRect.gameObject.SetActive(false);
                cooldownRect.gameObject.SetActive(true);
                cooldownText.text = $"{skill.applyCooldown:F2}초";
                break;
            case SkillType.Random:
                skillTypeText.text = "확률";
                skillPercentRect.gameObject.SetActive(true);
                cooldownRect.gameObject.SetActive(false);
                skillPercentText.text = $"{skill.applyPercentage:P1}";
                break;
        }
    }
}
