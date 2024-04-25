using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

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

    public void UpgradeCharacter(CharacterModel currentCharacter, CharacterModel targetCharacter)
    {
        // 기존 캐릭터를 새 캐릭터로 업그레이드

        int index = ownCharacters.FindIndex(0,(character) => character == currentCharacter);
        ownCharacters[index] = targetCharacter;
    }

    public void RemoveCharacter(CharacterModel removedCharacter)
    {
        wholeCharacters.Remove(removedCharacter);

        if (removedCharacter.photonView.IsMine)
        {
            ownCharacters.Remove(removedCharacter);
        }
    }

    private void SortList()
    {
        ownCharacters.Sort((a, b) => a.tier - b.tier);
    }
}