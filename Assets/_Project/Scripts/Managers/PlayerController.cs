using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }   

    public PlayerInput input;
    public List<CharacterModel> characters; // 현재 선택된 캐릭터들
    public CharacterModel mainCharacter;
    public bool showAttackRange;

    private void Awake()
    {
        Instance = this; 
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (characters.Count == 0)
        {
            return;
        }

        foreach (CharacterModel character in characters)
        {
            Vector3 direction = new Vector3(input.move.x, 0.0f, input.move.y);
            character.move.Move(direction);
        }
    }

    public void AddCharacter(CharacterModel character)
    {
        if (characters.Contains(character))
        {
            return;
        }

        if (mainCharacter != null)
        {
            mainCharacter.ui.Select(false);
        }

        character.agent.avoidancePriority = 50 + (10 - character.tier);
        character.agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        character.ui.Select(true, showAttackRange); 

        characters.Add(character);
        mainCharacter = character;

        UIManager.Instance.characterSelector.SelectCharacter(character);
        UIManager.Instance.ShowCharacterInfo(mainCharacter);
    }

    public void RemoveCharacter(CharacterModel character)
    {
        if (characters.Contains(character) == false)
        {
            return;
        }

        character.move.Move(Vector3.zero);
        character.agent.avoidancePriority = 50;
        character.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        character.ui.Deselect();

        UIManager.Instance.characterSelector.DeselectCharacter(character);
        characters.Remove(character);

        if (characters.Count > 0 
            && character == mainCharacter)
        {
            mainCharacter = characters[0];
            mainCharacter.ui.Select(true, showAttackRange);
        }
        else
        {
            mainCharacter = null;
            UIManager.Instance.HideCharacterInfo();
        }
    }

    public void SelectAllCharacters()
    {
        ResetCharacters();
        foreach (CharacterModel character in CharacterManager.Instance.ownCharacters)
        {
            AddCharacter(character);
        }

        if (mainCharacter != null)
        {
            mainCharacter.ui.Select(false);
            mainCharacter = characters[0];
            mainCharacter.ui.Select(true, showAttackRange);
            UIManager.Instance.ShowCharacterInfo(mainCharacter);
        }
    }

    public void ResetCharacters()
    {
        // 캐릭터 리스트를 비웁니다.

        foreach(CharacterModel character in characters)
        {
            character.move.Move(Vector3.zero);
            character.agent.avoidancePriority = 50;
            character.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            character.ui.Deselect();
            UIManager.Instance.characterSelector.DeselectCharacter(character);
            print($"{character.name} delete from list");
        }

        characters.Clear();
        mainCharacter = null;

        UIManager.Instance.HideCharacterInfo();
    }

    public void GenerateNewCharacter()
    {
        string classType = GameManager.Instance.defensePlayer.classType.ToString();

        switch (classType)
        {
            case "Tank":
                CharacterGenerator.Instance.GenerateCharacter(CharacterType.TankT1_Peasant);
                break;
            case "Deal":
                CharacterGenerator.Instance.GenerateCharacter(CharacterType.DealT1_Peasant);
                break;
            case "Heal":
                CharacterGenerator.Instance.GenerateCharacter(CharacterType.HealT1_Peasant);
                break;
        }
    }

    public void SetShowAttackRange()
    {
        showAttackRange = showAttackRange == false;

        if (mainCharacter != null)
        {
            mainCharacter.ui.Select(true, showAttackRange);
        }
    }

    public void AddCrystal(int color)
    {
        if (mainCharacter == null)
        {
            return;
        }

        mainCharacter.AddCrystal(color);
    }
}
