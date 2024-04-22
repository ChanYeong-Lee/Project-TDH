using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public List<CharacterModel> characters; // 현재 선택된 캐릭터들

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(x, y).normalized;

        foreach (CharacterModel character in characters)
        {
            character.move.Rotate(new Vector3(move.x, 0.0f, move.y));
            character.move.Move(new Vector3(move.x, 0.0f, move.y));
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
        
        characters.Clear();
    }
}
