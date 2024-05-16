using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectElement : MonoBehaviour
{
    [HideInInspector] public Button button;

    public Image characterIcon;
    public Image selectedIndicator;
    public CharacterModel character;

    public void Init()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        selectedIndicator.gameObject.SetActive(false);
    }
     
    public void SetCharacter(CharacterModel character)
    {
        if (character == null)
        {
            character = null;
            characterIcon.enabled = false;
        }
        else
        {
            this.character = character;
            characterIcon.enabled = true;   
            characterIcon.sprite = character.defaultStat.characterIcon;
        }
    }

    public void Select()
    {
        selectedIndicator.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        selectedIndicator.gameObject.SetActive(false);
    }
}
