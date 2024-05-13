using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInformationUI : MonoBehaviour
{
    [Header("설정")]
    public Transform characterPortraitCameraArm;

    public List<CrystalIndicator> crystalIndicators;
    public Button cancelButton;

    public RectTransform crystalButtons;
    public Button redCrystalButton;
    public Button greenCrystalButton;
    public Button blueCrystalButton;

    [Header("상태")]
    public CharacterModel model;
    public int currentIndex;

    private void Update()
    {
        if (model != null)
        {
            characterPortraitCameraArm.transform.position = model.transform.position;
            characterPortraitCameraArm.transform.rotation = model.transform.rotation;
        }
    }

    public void Init()
    {
        currentIndex = -1;

        cancelButton.onClick.AddListener(OnCancelButtonClick);

        redCrystalButton.onClick.AddListener(() => OnCrystalButtonClick(0));
        greenCrystalButton.onClick.AddListener(() => OnCrystalButtonClick(1));
        blueCrystalButton.onClick.AddListener(() => OnCrystalButtonClick(2));
        crystalButtons.gameObject.SetActive(false);

        for (int i = 0; i < crystalIndicators.Count; i++)
        {
            crystalIndicators[i].Init();
            crystalIndicators[i].button.onClick.AddListener(() => OnCrystalIndicatorButtonClick(i));
        }
    }

    public void SetCharacter(CharacterModel model)
    {
        this.model = model;

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
        if (model == null
            || currentIndex < 0)
        {
            return;
        }

        switch (color)
        {
            case 0:
                model.AddCrystal(0);
                break;
            case 1:
                model.AddCrystal(1);
                break;
            case 2:
                model.AddCrystal(2);
                break;
        }

        crystalIndicators[currentIndex].SetCrystal(color);
    }
}
