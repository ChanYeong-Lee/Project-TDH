using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInformationUI : MonoBehaviour
{
    [Header("설정")]
    public Image characterIcon;
    public CharacterStatUI characterStat;

    public List<SkillIndicator> skillIndicators;
    public SkillInformationUI skillInformation;

    public List<CrystalIndicator> crystalIndicators;
    public Button cancelButton;

    public RectTransform crystalButtons;
    public Button redCrystalButton;
    public Button greenCrystalButton;
    public Button blueCrystalButton;

    [Header("상태")]
    public CharacterModel model;
    public int currentIndex;

    public void Init()
    {
        currentIndex = -1;

        cancelButton.onClick.AddListener(OnCancelButtonClick);

        redCrystalButton.onClick.AddListener(() => OnCrystalButtonClick(0));
        greenCrystalButton.onClick.AddListener(() => OnCrystalButtonClick(1));
        blueCrystalButton.onClick.AddListener(() => OnCrystalButtonClick(2));
        crystalButtons.gameObject.SetActive(false);
        skillInformation.gameObject.SetActive(false);

        crystalIndicators[0].Init();
        crystalIndicators[1].Init();
        crystalIndicators[2].Init();

        crystalIndicators[0].button.onClick.AddListener(()=> OnCrystalIndicatorButtonClick(0));
        crystalIndicators[1].button.onClick.AddListener(() => OnCrystalIndicatorButtonClick(1));
        crystalIndicators[2].button.onClick.AddListener(() => OnCrystalIndicatorButtonClick(2));
    }

    public void SetCharacter(CharacterModel model)
    {
        this.model = model;
        characterStat.SetCharacter(model);

        currentIndex = -1;
        crystalButtons.gameObject.SetActive(false);

        if (model == null)
        {
            for (int i = 0; i <crystalIndicators.Count; i++)
            {
                crystalIndicators[i].SetCrystal(-2);
            }
            return;
        }

        characterIcon.sprite = model.defaultStat.characterIcon;

        crystalIndicators[0].SetCrystal(-1);
        crystalIndicators[1].SetCrystal(-1);
        switch (model.tier)
        {
            case 1:
                crystalIndicators[2].SetCrystal(-2);
                break;
            case 2:
            case 3:
                crystalIndicators[2].SetCrystal(-1);
                break;
        }

        for (int i = 0; i < model.crystals.Count; i++)
        {
            crystalIndicators[i].SetCrystal(model.crystals[i]);
        }

        for (int i = 0; i < skillIndicators.Count; i++)
        {
            print($"Set {model.defaultStat.characterName} Skills {i} has {model.skill.skills.Count} Skills");
            if (model.skill.skills.Count > i)
            {
                skillIndicators[i].SetSkill(model.skill.skills[i]);
            }
            else
            {
                skillIndicators[i].SetSkill(null);
            }
        }
    }

    private void OnCancelButtonClick()
    {
        if (currentIndex >= 0)
        {
            crystalButtons.DOScale(Vector3.zero, 0.1f).OnComplete(() => crystalButtons.gameObject.SetActive(false));
            currentIndex = -1;
        }
    }

    private void OnCrystalIndicatorButtonClick(int index)
    {
        if (model == null)
        {
            return;
        }
        
        currentIndex = index;

        crystalButtons.gameObject.SetActive(true);
        crystalButtons.anchoredPosition = new Vector2(crystalIndicators[index].rectTransform.anchoredPosition.x, 20.0f);
        crystalButtons.DOScale(Vector3.one, 0.1f);
    }

    private void OnCrystalButtonClick(int color)
    {
        Queue<int> a= new Queue<int>();
        
        if (model == null || currentIndex < 0)
        {
            return;
        }

        DefensePlayer player = GameManager.Instance.defensePlayer;

        if (player.UseCrystal(color))
        {
            crystalIndicators[currentIndex].SetCrystal(color);
            model.AddCrystal(color);
        }

        crystalButtons.DOScale(Vector3.zero, 0.1f).OnComplete(() => crystalButtons.gameObject.SetActive(false));
        currentIndex = -1;
    }
}
