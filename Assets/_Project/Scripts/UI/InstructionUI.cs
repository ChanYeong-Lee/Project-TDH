using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionUI : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.1f);
    }

    public void Hide()
    {
        transform.DOScale(Vector3.zero, 0.1f).OnComplete(() => gameObject.SetActive(false));
    }
}