using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 move;
    public Joystick joystick;

    private void Update()
    {
        InputMove();
    }

    private void InputMove()
    {
        float x = joystick.Horizontal;
        float y = joystick.Vertical;

        if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
        {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
        }

        move = new Vector2(x, y).normalized;
    }
}
