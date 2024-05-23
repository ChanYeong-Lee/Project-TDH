using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkillInformationUI informationUI;

    public Image skillIcon;
    public Image fillImage;

    [Header("ป๓ลย")]
    public Skill skill;

    private void Update()
    {
        if (skill == null)
        {
            return;
        }

        if (skill.defaultStat.skillType == SkillType.NonTargetCooldown
            || skill.defaultStat.skillType == SkillType.TargetCooldown)
        {
            fillImage.fillAmount = 1.0f - skill.coolDownAmount;
        }
    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;

        if (skill == null)
        {
            skillIcon.sprite = null;
            skillIcon.enabled = false;
            fillImage.enabled = false;
            return;
        }

        skillIcon.enabled = true;
        skillIcon.sprite = skill.defaultStat.skillIcon;

        fillImage.enabled = skill.defaultStat.skillType == SkillType.NonTargetCooldown
            || skill.defaultStat.skillType == SkillType.TargetCooldown;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skill == null)
        {
            return;
        }

        informationUI.SetSkill(skill);
        informationUI.gameObject.SetActive(true);

        skill.owner.GetComponent<CharacterModel>().ui.ShowSkillArea(skill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skill == null)
        {
            return;
        }

        informationUI.SetSkill(null);
        informationUI.gameObject.SetActive(false);

        skill.owner.GetComponent<CharacterModel>().ui.HideSkillArea();
    }
}
