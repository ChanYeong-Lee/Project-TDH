using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectorUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("설정")]
    public float moveDuration = 0.1f;
    
    public Button deselectAllButton;
    
    public RectTransform activeElements;

    public Button openButton;
    public Button closeButton;

    public Button selectAllButton;
    public Button leftButton;
    public Button rightButton;

    public RectTransform content;
    public CharacterSelectElement characterElementPrefab;

    [Header("상태")]
    public List<CharacterSelectElement> characterElements;

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

        deselectAllButton.onClick.AddListener(OnDeselectAllButtonClick);
        selectAllButton.onClick.AddListener(OnSelectAllButtonClick);
        leftButton.onClick.AddListener(MovePageLeft);
        rightButton.onClick.AddListener(MovePageRight);

        for (int i = 0; i < 12; i++)
        {
            CharacterSelectElement element = PoolManager.Instance.clientPool.Spawn(characterElementPrefab.gameObject, content).GetComponent<CharacterSelectElement>();
            element.Init();
            element.button.onClick.AddListener(() => OnCharacterButtonClick(element));
            characterElements.Add(element);
            element.button.interactable = false;
        }
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
        for (int i = 0; i < 12; i++)
        {
            if (characterList.Count > i)
            {
                characterElements[i].SetCharacter(characterList[i]);
                characterElements[i].button.interactable = true;
            }
            else
            {
                characterElements[i].SetCharacter(null);
                characterElements[i].button.interactable = false;
            }
        }

        leftButton.interactable = page > 0;
        rightButton.interactable = page < lastPage;
    }

    public void SelectCharacter(CharacterModel character)
    {
        if (character == null)
        {
            return;
        }

        CharacterSelectElement element = characterElements.Find((element) => element.character == character);

        if (element != null)
        {
            element.Select();
        }
    }

    public void DeselectCharacter(CharacterModel character)
    {
        if (character == null)
        {
            return;
        }

        CharacterSelectElement element = characterElements.Find((element) => element.character == character);

        if (element != null)
        {
            element.Deselect();
        }
    }

    public void OnDeselectAllButtonClick()
    {
        PlayerController.Instance.ResetCharacters();
    }

    public void OnSelectAllButtonClick()
    {
        PlayerController.Instance.SelectAllCharacters();
    }

    public void OnCharacterButtonClick(CharacterSelectElement element)
    {
        if (PlayerController.Instance.characters.Contains(element.character))
        {
            PlayerController.Instance.RemoveCharacter(element.character);
        }
        else
        {
            PlayerController.Instance.AddCharacter(element.character);
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