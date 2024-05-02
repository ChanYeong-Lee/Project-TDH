using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void UpgradeCharacter(CharacterModel currentCharacter, CharacterType targetCharacterType)
    {
        // ���� ĳ���͸� �� ĳ���ͷ� ���׷��̵�
        Vector3Int generationCrystals = currentCharacter.crystals;
        CharacterModel targetCharacter = CharacterGenerator.Instance.GenerateCharacter(targetCharacterType);
        targetCharacter.SetGenerationCrystals(generationCrystals);

        RemoveCharacter(currentCharacter);
    }

    public void RemoveCharacter(CharacterModel removedCharacter)
    {
        if (wholeCharacters.Contains(removedCharacter))
        {
            wholeCharacters.Remove(removedCharacter);
        }

        if (removedCharacter.photonView.IsMine)
        {
            ownCharacters.Remove(removedCharacter);
            if (PlayerController.Instance.characters.Contains(removedCharacter))
            {
                PlayerController.Instance.characters.Remove(removedCharacter);
            }
        }

        PhotonNetwork.Destroy(removedCharacter.gameObject);
    }

    private void SortList()
    {
        ownCharacters.Sort((a, b) => a.tier - b.tier);
    }
}