using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public List<CharacterModel> characters; // �ʿ� �����ϴ� ��� �ڽ��� ĳ���͵�

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateCharacter(CharacterModel model)
    {
        // ���ο� ĳ���� ����

        characters.Add(model);
    }
    
    public void UpgradeCharacter(CharacterModel currentCharacter, CharacterModel targetCharacter)
    {
        // ���� ĳ���͸� �� ĳ���ͷ� ���׷��̵�

        int index = characters.FindIndex(0,(character) => character == currentCharacter);
        characters[index] = targetCharacter;

    }
}