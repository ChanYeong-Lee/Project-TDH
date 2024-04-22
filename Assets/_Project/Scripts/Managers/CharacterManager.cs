using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public List<CharacterModel> wholeCharacters; // 맵에 존재하는 모든 캐릭터들
    public List<CharacterModel> characters; // 맵에 존재하는 모든 자신의 캐릭터들

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateCharacter(CharacterType type)
    {
        // 새로운 캐릭터 생성
        CharacterModel newModel = PhotonNetwork.Instantiate("",Vector3.zero,Quaternion.identity).GetComponent<CharacterModel>();
        characters.Add(newModel);
        SortList();
    }
    
    public void UpgradeCharacter(CharacterModel currentCharacter, CharacterModel targetCharacter)
    {
        // 기존 캐릭터를 새 캐릭터로 업그레이드

        int index = characters.FindIndex(0,(character) => character == currentCharacter);
        characters[index] = targetCharacter;
    }

    private void SortList()
    {
        characters.Sort((a, b) => a.tier - b.tier);
    }
}