using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public List<CharacterModel> characters; // ���� ���õ� ĳ���͵�

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
        // ����� ���� ĳ���͸� �߰��մϴ�.

        characters.Add(character);
    }

    public void ResetCharacter()
    {
        // ĳ���� ����Ʈ�� ���ϴ�.
        
        characters.Clear();
    }
}
