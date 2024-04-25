using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }   

    public PlayerInput input;
    public List<CharacterModel> characters; // 현재 선택된 캐릭터들

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
        // 명령을 내릴 캐릭터를 추가합니다.

        characters.Add(character);
    }

    public void ResetCharacter()
    {
        // 캐릭터 리스트를 비웁니다.

        foreach (CharacterModel character in characters)
        {
            character.move.Move(Vector3.zero);
        }

        characters.Clear();
    }
}
