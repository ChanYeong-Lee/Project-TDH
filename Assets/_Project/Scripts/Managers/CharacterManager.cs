using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }
    public List<CharacterModel> wholeCharacters; // 맵에 존재하는 모든 캐릭터들
    public List<CharacterModel> ownCharacters; // 맵에 존재하는 모든 자신의 캐릭터들

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
        // 기존 캐릭터를 새 캐릭터로 업그레이드
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