using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllignPrefabs : MonoBehaviour
{
    public GameObject footHoldPrefab;
    public Transform footHoldParent;
    public Transform prefabParent;
    public List<Transform> prefabs;
    public float padding = 1.0f;

    [ContextMenu("Allign")]
    public void Allign()
    {
        if (footHoldParent != null)
        {
            DestroyImmediate(footHoldParent.gameObject);
        }

        footHoldParent = new GameObject("FootHoldParent").transform;
        footHoldParent.SetParent(transform);
        footHoldParent.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);  

        for (int i = 0; i < prefabs.Count; i++)
        {
            prefabs[i].SetParent(prefabParent);
            prefabs[i].SetSiblingIndex(i);
            prefabs[i].position = transform.position + (-padding * i) * Vector3.right + (2 * footHoldPrefab.transform.localScale.y) * Vector3.up;
            GameObject footHoldInstance = Instantiate(footHoldPrefab, prefabs[i]);
            footHoldInstance.transform.localPosition = (-footHoldInstance.transform.localScale.y) * Vector3.up;
            footHoldInstance.transform.SetParent(footHoldParent);
        }
    }
}
