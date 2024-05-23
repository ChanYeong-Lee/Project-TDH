using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    public Image skillRangeImage;
    private Skill currentRangeSkill;

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

        coolDownFillImage.gameObject.SetActive(coolDownSkill != null);
        skillRangeImage.gameObject.SetActive(false);
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

    public void ShowSkillArea(Skill skill)
    {
        if (skill == null)
        {
            return;
        }
        
        UpdateSkillArea(skill);
    }

    public void HideSkillArea()
    {
        skillRangeImage.gameObject.SetActive(false);
    }

    private void UpdateSkillArea(Skill skill)
    {
        if (skill == null)
        {
            return;
        }

        bool haveArea = true;

        switch (skill.defaultStat.skillType)
        {
            case SkillType.Always:
                switch (skill.defaultStat.mainTargetType)
                {
                    case TargetType.Enemy:
                        skillRangeImage.color = new Color(1.0f, 0.0f, 0.0f, 0.1f);
                        break;
                    case TargetType.AllAlly:
                        skillRangeImage.color = new Color(0.0f, 1.0f, 0.0f, 0.1f);
                        break;

                    default:
                        haveArea = false;
                        break;
                }
                break;
            case SkillType.NonTargetCooldown:
            case SkillType.TargetCooldown:
                switch (skill.defaultStat.mainTargetType)
                {
                    case TargetType.AllAlly:
                    case TargetType.StrongestAlly:
                        skillRangeImage.color = new Color(0.0f, 1.0f, 0.0f, 0.1f);
                        break;
                    default:
                        haveArea = false;
                        break;
                }
                break;
            default:
                haveArea = false;
                break;
        }

        if (haveArea)
        {
            skillRangeImage.gameObject.SetActive(true);

            foreach (BuffEffect buff in skill.buffEffects)
            {
                skillRangeImage.transform.localScale = buff.attackArea * model.attack.attackAreaIncrease * Vector3.one;
            }
        }
        else
        {
            skillRangeImage.gameObject.SetActive(false);
        }
    }
}