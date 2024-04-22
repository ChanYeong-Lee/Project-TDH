using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBrain : MonoBehaviour
{
    public CharacterModel model;

    private void Awake()
    {
        model = GetComponent<CharacterModel>();
    }

    private void Update()
    {

    }
}
