using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public List<CharacterModel> wholeCharacters; // �ʿ� �����ϴ� ��� ĳ���͵�
    public List<CharacterModel> characters; // �ʿ� �����ϴ� ��� �ڽ��� ĳ���͵�

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateCharacter(CharacterType type)
    {
        // ���ο� ĳ���� ����
        CharacterModel newModel = PhotonNetwork.Instantiate("",Vector3.zero,Quaternion.identity).GetComponent<CharacterModel>();
        characters.Add(newModel);
        SortList();
    }
    
    public void UpgradeCharacter(CharacterModel currentCharacter, CharacterModel targetCharacter)
    {
        // ���� ĳ���͸� �� ĳ���ͷ� ���׷��̵�

        int index = characters.FindIndex(0,(character) => character == currentCharacter);
        characters[index] = targetCharacter;
    }

    private void SortList()
    {
        characters.Sort((a, b) => a.tier - b.tier);
    }
}