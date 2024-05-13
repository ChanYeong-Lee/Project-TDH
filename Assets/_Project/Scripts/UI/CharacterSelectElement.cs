using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectElement : MonoBehaviour
{
    [HideInInspector] public Button button;
    [HideInInspector] public TMP_Text nameText;

    public CharacterModel character;

    public void Init()
    {
        button = GetComponent<Button>();
        nameText = GameObject.Find("NameText").GetComponent<TMP_Text>();
    }

    public void SetCharacter(CharacterModel character)
    {
        this.character = character;
        nameText.text = character.name;
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
