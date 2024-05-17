using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

            PlayerController.Instance.ResetCharacters();
            PlayerController.Instance.AddCharacter(newCharacter);
        }
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
                PlayerController.Instance.RemoveCharacter(removedCharacter);
            }
        }
    }

    public void UpgradeCharacter(CharacterModel currentCharacter, CharacterType targetCharacterType)
    {
        List<int> generationCrystals = currentCharacter.crystals;
        Vector3 spawnPosition = currentCharacter.transform.position;

        ownCharacters.Remove(currentCharacter);
        if (PlayerController.Instance.characters.Contains(currentCharacter))
        {
            PlayerController.Instance.characters.Remove(currentCharacter);
        }

        CharacterGenerator.Instance.RemoveCharacter(currentCharacter);
        CharacterModel targetCharacter = CharacterGenerator.Instance.GenerateCharacter(targetCharacterType);

        targetCharacter.transform.position = spawnPosition;
        targetCharacter.SetGenerationCrystals(generationCrystals);
    }

    private void SortList()
    {
        print("Sort List");
        ownCharacters.Sort((a, b) => b.tier - a.tier);
        UIManager.Instance.characterSelector.SetList(ownCharacters);
    }
}