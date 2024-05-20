using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public bool multipleSelect { get; set; }
    public bool showAttackRange;

    public PlayerInput input;
    public List<CharacterModel> characters; // 현재 선택된 캐릭터들
    public CharacterModel mainCharacter;

    public GameObject multiSelectButton;
    public GameObject multiSelectToggle;
    private void Awake()
    {
        Instance = this;
        input = GetComponent<PlayerInput>();

        if (Application.isMobilePlatform)
        {
            multiSelectButton.gameObject.SetActive(true);
            multiSelectToggle.gameObject.SetActive(false);
        }
        else
        {
            multiSelectButton.gameObject.SetActive(false);
            multiSelectToggle.gameObject.SetActive(true);
        }
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
        if (characters.Contains(character) && multipleSelect)
        {
            return;
        }

        if (multipleSelect == false)
        {
            ResetCharacters();
        }

        if (mainCharacter != null)
        {
            mainCharacter.ui.Select(false);
        }

        print($"Player Controller : Add Character ({character.defaultStat.characterName})");
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

        print($"Player Controller : Remove Character ({character.defaultStat.characterName})");

        character.move.Move(Vector3.zero);
        character.agent.avoidancePriority = 50;
        character.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        character.ui.Deselect();

        UIManager.Instance.characterSelector.DeselectCharacter(character);
        characters.Remove(character);

        if (characters.Count > 0)
        {
            if (mainCharacter == character)
            {
                mainCharacter = characters[0];
                mainCharacter.ui.Select(true, showAttackRange);
                UIManager.Instance.ShowCharacterInfo(mainCharacter);
            }
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
            print($"{character.defaultStat.characterName} delete from list");
        }

        characters.Clear();
        mainCharacter = null;

        UIManager.Instance.HideCharacterInfo();
    }

    public void GenerateNewCharacter()
    {
        DefensePlayer player = GameManager.Instance.defensePlayer;
    
        if (player.singlePlay)
        {
            int randomInteger = Random.Range(0, 3) * 10;
            CharacterGenerator.Instance.GenerateCharacter((CharacterType)randomInteger);
        }
        else
        {
            switch (player.classType)
            {
                case ClassType.Tank:
                    CharacterGenerator.Instance.GenerateCharacter(CharacterType.TankT1_Peasant);
                    break;
                case ClassType.Deal:
                    CharacterGenerator.Instance.GenerateCharacter(CharacterType.DealT1_Peasant);
                    break;
                case ClassType.Heal:
                    CharacterGenerator.Instance.GenerateCharacter(CharacterType.HealT1_Peasant);
                    break;
            }
        } 
    }

    public void SetShowAttackRange(bool isOn)
    {
        showAttackRange = isOn;

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
