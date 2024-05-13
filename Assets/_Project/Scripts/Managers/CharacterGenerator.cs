using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    private const string prefabPath = "Prefabs/Characters/";
    public static CharacterGenerator Instance { get; private set; }
    public List<CharacterModel> characterPrefabs;

    private void Awake()
    {
        Instance = this;
    }

    public CharacterModel GenerateCharacter(CharacterType type)
    {
        CharacterModel prefab = characterPrefabs.Find((model) => model.type == type);
        string prefabName = prefab.name;

        CharacterModel newModel = PhotonNetwork.Instantiate(prefabPath + prefabName, Vector3.zero, Quaternion.identity).GetComponent<CharacterModel>();

        //PlayerController.Instance.ResetCharacter();
        PlayerController.Instance.AddCharacter(newModel);
        return newModel;
    }

    public void GenerateNewCharacter()
    {
        CharacterModel prefab = null;
        string classType = GameManager.Instance.defensePlayer.classType.ToString();
        switch (classType)
        {
            case "Tank":
                break;
            case "Deal":
                break;
            case "Heal":
                break;
        }
    }
}