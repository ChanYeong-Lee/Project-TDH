using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public RectTransform bossHealthBar;
    public Image bossHealthBarFillImage;

    private void Awake()
    {
        Instance = this;
    }
}