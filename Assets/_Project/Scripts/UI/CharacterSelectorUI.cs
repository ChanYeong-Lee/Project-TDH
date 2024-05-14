using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectorUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float moveDuration = 0.1f;
    
    public RectTransform activeElements;
    public Button openButton;
    public Button closeButton;

    public RectTransform content;
    public CharacterSelectElement characterElementPrefab;

    public List<CharacterSelectElement> characterElements;

    public Button leftButton;
    public Button rightButton;

    public int page;
    public int lastPage => (characterElements.Count - 1) / 4;
    public Vector2 pagePosition => new Vector2(-page * 520.0f, 0.0f);

    private RectTransform rectTransform;
    private Vector2 originPos;
    private Vector2 disablePos;

    public void Init()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        originPos = rectTransform.anchoredPosition;
        disablePos = new Vector2(0.0f, originPos.y);

        openButton.onClick.AddListener(Show);
        closeButton.onClick.AddListener(Hide);
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

    public void SetList(List<CharacterModel> characterList)
    {
        foreach (RectTransform child in content)
        {
            Destroy(child.gameObject);
        }
        characterElements.Clear();

        foreach (CharacterModel character in characterList)
        {
            CharacterSelectElement instance = PoolManager.Instance.clientPool.Spawn(characterElementPrefab.gameObject, content).GetComponent<CharacterSelectElement>();
            instance.Init();
            instance.SetCharacter(character);
        }
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
        if (originPos.x < rectTransform.anchoredPosition.x + eventData.delta.x
            && rectTransform.anchoredPosition.x + eventData.delta.x < 0.0f)
        {
            transform.position += new Vector3(eventData.delta.x, 0.0f, 0.0f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (activeElements.gameObject.activeSelf)
        {
            if (rectTransform.anchoredPosition.x < originPos.x * 0.7f)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
        else
        {
            if (rectTransform.anchoredPosition.x < originPos.x * 0.3f)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        
    }

    public void Show()
    {
        activeElements.gameObject.SetActive(true);
        openButton.gameObject.SetActive(false);

        rectTransform.DOAnchorPos(originPos, 0.15f);
    }

    public void Hide()
    {
        rectTransform.DOAnchorPos(disablePos, 0.15f).OnComplete(() =>
        {
            activeElements.gameObject.SetActive(false);
            openButton.gameObject.SetActive(true);
        });
    }
}