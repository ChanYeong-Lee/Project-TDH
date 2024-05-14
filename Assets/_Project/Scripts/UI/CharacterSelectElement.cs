using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectElement : MonoBehaviour
{
    [HideInInspector] public Button button;

    public Image characterIcon;
    public CharacterModel character;

    public void Init()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void SetCharacter(CharacterModel character)
    {
        this.character = character;
        characterIcon.sprite = character.defaultStat.characterIcon;
    }

    public void OnClick()
    {
        if (character == null)
        {
            return;
        }
        PlayerController.Instance.AddCharacter(character);
    }
}
