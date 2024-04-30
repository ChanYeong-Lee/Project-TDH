using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }
    public List<CharacterModel> wholeCharacters; // �ʿ� �����ϴ� ��� ĳ���͵�
    public List<CharacterModel> ownCharacters; // �ʿ� �����ϴ� ��� �ڽ��� ĳ���͵�

    private void Awake()
    {
        Instance = this;
    }

    public void AddCharacter(CharacterModel newCharacter)
    {
        wholeCharacters.Add(newCharacter);

        if (newCharacter.photonView.IsMine)
        {
            ownCharacters.Add(newCharacter);
            SortList();
        }
    }

    public void UpgradeCharacter(CharacterModel currentCharacter, CharacterModel targetCharacter)
    {
        // ���� ĳ���͸� �� ĳ���ͷ� ���׷��̵�

        
    }

    public void RemoveCharacter(CharacterModel removedCharacter)
    {
        wholeCharacters.Remove(removedCharacter);

        if (removedCharacter.photonView.IsMine)
        {
            ownCharacters.Remove(removedCharacter);
            if (PlayerController.Instance.characters.Contains(removedCharacter))
            {
                PlayerController.Instance.characters.Remove(removedCharacter);
            }
        }

        Destroy(removedCharacter.gameObject);
    }

    private void SortList()
    {
        ownCharacters.Sort((a, b) => a.tier - b.tier);
    }
}