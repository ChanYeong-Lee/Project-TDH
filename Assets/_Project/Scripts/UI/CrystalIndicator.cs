using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrystalIndicator : MonoBehaviour
{
    [HideInInspector] public RectTransform rectTransform;

    public Button button;
    public Image icon;
    
    private Color noneColor;
    private Color emptyColor;
    private Color redColor;
    private Color greenColor;
    private Color blueColor;

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();

        noneColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        emptyColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        redColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        greenColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        blueColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
    }

    public void SetCrystal(int color)
    {
        switch (color)
        {
            case -2:
                button.interactable = false;
                icon.color = noneColor;
                break;
            case -1:
                button.interactable = true;
                icon.color = emptyColor;
                break;
            case 0:
                button.interactable = false;
                icon.color = redColor;
                break;
            case 1:
                button.interactable = false;
                icon.color = greenColor;
                break;
            case 2:
                button.interactable = false;
                icon.color = blueColor;
                break;
        }
    }
}
