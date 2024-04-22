using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInput input;
    public List<CharacterModel> characters; // ���� ���õ� ĳ���͵�

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        foreach (CharacterModel character in characters)
        {
            Vector3 direction = new Vector3(input.move.x, 0.0f, input.move.y);
            character.move.Move(direction);
            character.move.Rotate(direction);
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

        foreach (CharacterModel character in characters)
        {
            character.move.Move(Vector3.zero);
        }

        characters.Clear();
    }
}
