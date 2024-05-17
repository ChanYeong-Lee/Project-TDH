using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterGenerator : MonoBehaviour
{
    private const string prefabPath = "Prefabs/Characters/";
    public static CharacterGenerator Instance { get; private set; }
    public List<CharacterModel> characterPrefabs;

    public Action<CharacterModel> onGenerateCharacter;

    private void Awake()
    {
        Instance = this;
    }

    public CharacterModel GenerateCharacter(CharacterType type)
    {
        CharacterModel prefab = characterPrefabs.Find((model) => model.type == type);
        string prefabName = prefab.name;

        Vector3 spawnPosition = new Vector3(-2.0f, 0.0f, 2.0f);
        Quaternion spawnRotation = Quaternion.identity;

        while (Physics.Raycast(spawnPosition + 100.0f * Vector3.up, Vector3.down, out RaycastHit hit, LayerMask.GetMask("Character")))
        {
            if (spawnPosition.x >= 2.0f)
            {
                spawnPosition.x = -2.0f;
                spawnPosition.z -= hit.collider.bounds.size.z;
            }
            else
            {
                spawnPosition += new Vector3(hit.collider.bounds.size.x, 0.0f, 0.0f);
            }
        }

        CharacterModel newModel = PhotonNetwork.Instantiate(prefabPath + prefabName, spawnPosition, spawnRotation).GetComponent<CharacterModel>();

        return newModel;
    }

    public void RemoveCharacter(CharacterModel model)
    {
        PhotonNetwork.Destroy(model.gameObject);
    }
}