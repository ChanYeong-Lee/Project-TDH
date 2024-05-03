using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectorUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float moveDuration = 0.1f;
    public RectTransform content;
    public List<Button> characterButtons;

    public Button leftButton;
    public Button rightButton;

    public int page;
    public int lastPage => (characterButtons.Count - 1)/4;
    public Vector2 pagePosition => new Vector2(-page * 520.0f, 0.0f);

    private void Awake()
    {
        leftButton.onClick.AddListener(MovePageLeft);
        rightButton.onClick.AddListener(MovePageRight);
    }

    private void OnEnable()
    {
        page = 0;
        content.anchoredPosition = pagePosition;
        leftButton.interactable = page > 0;
        rightButton.interactable = page < lastPage;
    }

    public void MovePageRight()
    {
        page++;
        content.DOAnchorPos(pagePosition, moveDuration);
        //content.anchoredPosition = pagePosition;
        leftButton.interactable = page > 0;
        rightButton.interactable = page < lastPage;
    }

    public void MovePageLeft()
    {
        page--;
        content.DOAnchorPos(pagePosition, moveDuration);
        //content.anchoredPosition = pagePosition;
        leftButton.interactable = page > 0;
        rightButton.interactable = page < lastPage;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position += new Vector3(eventData.delta.x, 0.0f, 0.0f);
    }


    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
