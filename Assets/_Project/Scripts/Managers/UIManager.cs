using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public CharacterInformationUI characterInfo;
    public CharacterSelectorUI characterSelector;

    public RectTransform bossHealthBar;
    public Image bossHealthBarFillImage;

    public PlayerCrystalsUI playerCrystals;

    private void Awake()
    {
        Instance = this;

        characterSelector.Init();
        characterInfo.Init();

        characterInfo.gameObject.SetActive(false);
    }

    public void ShowCharacterInfo(CharacterModel model)
    {
        characterInfo.gameObject.SetActive(true);
        characterInfo.SetCharacter(model);
    }

    public void HideCharacterInfo()
    {
        characterInfo.SetCharacter(null);
        characterInfo.gameObject.SetActive(false);
    }
}