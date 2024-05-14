using Photon.Pun;
using System.Collections.Generic;
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
            RemoveCharacter(character);
            return;
        }

        // 명령을 내릴 캐릭터를 추가합니다.
        if (mainCharacter != null)
        {
            mainCharacter.ui.Select(false);
        }

        character.agent.avoidancePriority = 50 + (10 - character.tier);
        character.agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        characters.Add(character);

        mainCharacter = character;
        character.ui.Select(true, showAttackRange);

        UIManager.Instance.ShowCharacterInfo(mainCharacter);
    }

    public void RemoveCharacter(CharacterModel character)
    {
        character.move.Move(Vector3.zero);
        character.agent.avoidancePriority = 50;
        character.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        character.ui.Deselect();

        characters.Remove(character);

        if (characters.Count > 0)
        {
            mainCharacter = characters[0];
        }
        else
        {
            mainCharacter = null;
            UIManager.Instance.HideCharacterInfo();
        }
    }

    public void ResetCharacter()
    {
        // 캐릭터 리스트를 비웁니다.

        foreach(CharacterModel character in characters)
        {
            character.move.Move(Vector3.zero);
            character.agent.avoidancePriority = 50;
            character.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            character.ui.Deselect();
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

    public void SelectAllCharacter()
    {
        foreach (CharacterModel character in CharacterManager.Instance.ownCharacters)
        {
            AddCharacter(character);
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
