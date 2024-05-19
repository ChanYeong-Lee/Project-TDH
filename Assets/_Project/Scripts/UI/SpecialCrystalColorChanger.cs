using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpecialCrystalColorChanger : MonoBehaviour
{
    private Image image;
    private int currentIndex;
    private Color currentColor = new Color(0.0f, 0.0f, 0.0f, 100.0f / 255.0f);

    public float changeTime;
    private float changeTimeDelta;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        changeTimeDelta -= Time.deltaTime;
        if (changeTimeDelta < 0)
        {
            currentIndex = (currentIndex + 1) % 3;
            changeTimeDelta = changeTime;
        }

        switch (currentIndex)
        {
            case 0:
                currentColor.r = Mathf.Sin(changeTimeDelta / changeTime * Mathf.PI);
                break;
            case 1:
                currentColor.g = Mathf.Sin(changeTimeDelta / changeTime * Mathf.PI);
                break;
            case 2:
                currentColor.b = Mathf.Sin(changeTimeDelta / changeTime * Mathf.PI);
                break;
        }

        image.color = currentColor;
    }
}
