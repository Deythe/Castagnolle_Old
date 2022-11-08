using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Page3 : MonoBehaviour
{
    public static Page3 instance;
    [SerializeField] private Transform cardMovable;
    [SerializeField] private Image cardMovableSprite;
    [SerializeField] private bool isTouchingACard;
    [SerializeField] private int currentIndexCard = -1;
    [SerializeField] private List<int> cardsInDeck = new List<int>();
    [SerializeField] private List<Image> cardsInDeckSprite;
    [SerializeField] private Transform parentCarteSprite;
    [SerializeField] private bool inDropZone;
    [SerializeField] private Image bigCard;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private TMP_Text countCardInDeck;

    private Image pivotImage;
    private WaitForSeconds timer = new WaitForSeconds(0.5f);
    private Coroutine currentCoroutine = null;
    private UnityEngine.Events.UnityAction buttonCallback;

    public bool p_isTouchingACard
    {
        get => isTouchingACard;
        set
        {
            isTouchingACard = value;
            if (isTouchingACard)
            {
                currentCoroutine = StartCoroutine(ShowBigCart());
            }
            else
            {
                StopCoroutine(currentCoroutine);
                bigCard.enabled = false;
            }
        }
    }

    public Image p_cardMovableSprite
    {
        get => cardMovableSprite;
        set
        {
            cardMovableSprite = value;
        }
    }

    public int p_currentIndexCard
    {
        get => currentIndexCard;
        set
        {
            currentIndexCard = value;
        }
    }
    
    public bool p_inDropZone
    {
        get => inDropZone;
        set
        {
            inDropZone = value;
        }
    }
    
    public Image p_bigCard
    {
        get => bigCard;
        set
        {
            bigCard = value;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ResetButtonActionCardInDeck();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (isTouchingACard)
            {
                if (Input.GetTouch(0).deltaPosition.x>=17 || Input.GetTouch(0).deltaPosition.x<=-17 && (Input.GetTouch(0).deltaPosition.y>=-5 && Input.GetTouch(0).deltaPosition.y<=5))
                {
                    scrollRect.vertical = false;
                    bigCard.enabled = false;
                    cardMovableSprite.enabled = true;
                    StopCoroutine(currentCoroutine);
                }
                
                cardMovable.position = Input.GetTouch(0).position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (inDropZone && currentIndexCard!=-1 && !cardsInDeck.Contains(currentIndexCard) && cardsInDeck.Count<8)
                {
                    PutCardInDeck();
                }
                else
                {
                    isTouchingACard = false;
                    cardMovableSprite.enabled = false;
                    currentIndexCard = -1;
                }
                
                scrollRect.vertical = true;
                StopCoroutine(currentCoroutine);
                bigCard.enabled = false;
                inDropZone = false;
            }
        }
    }

    public void PutCardInDeck()
    {
        cardsInDeck.Add(currentIndexCard);
        foreach (var sprite in cardsInDeckSprite)
        {
            if (!sprite.enabled)
            {
                sprite.sprite = cardMovableSprite.sprite;
                sprite.enabled = true;
                currentIndexCard = -1;
                isTouchingACard = false;
                cardMovableSprite.enabled = false;
                countCardInDeck.text = cardsInDeck.Count + " / 8";
                return;
            }
        }
    }

    public void ResetButtonActionCardInDeck()
    {
        for (int j = 0; j < parentCarteSprite.childCount; j++)
        {
            parentCarteSprite.GetChild(j).GetComponent<Button>().onClick.RemoveAllListeners();
            switch (j)
            {
                case 0:
                    buttonCallback = ()=> RemoveCardFromDeck(0);
                    break;
                case 1:
                    buttonCallback = ()=> RemoveCardFromDeck(1);
                    break;
                case 2:
                    buttonCallback = ()=> RemoveCardFromDeck(2);
                    break;
                case 3:
                    buttonCallback = ()=> RemoveCardFromDeck(3);
                    break;
                case 4:
                    buttonCallback = ()=> RemoveCardFromDeck(4);
                    break;
                case 5:
                    buttonCallback = ()=> RemoveCardFromDeck(5);
                    break;
                case 6:
                    buttonCallback = ()=> RemoveCardFromDeck(6);
                    break;
                case 7:
                    buttonCallback = ()=> RemoveCardFromDeck(7);
                    break;
            }
            parentCarteSprite.GetChild(j).GetComponent<Button>().onClick.AddListener(buttonCallback);
        }
    }
    
    public void RemoveCardFromDeck(int i)
    {
        cardsInDeck.RemoveAt(i);
        for (int j = 0; j < parentCarteSprite.childCount; j++)
        {
            if (j.Equals(i))
            {
                cardsInDeckSprite.RemoveAt(j);
                cardsInDeckSprite[j].enabled = false;
                pivotImage = cardsInDeckSprite[j];
                cardsInDeckSprite.RemoveAt(j);
                cardsInDeckSprite.Add(pivotImage);
                pivotImage = null;
                parentCarteSprite.GetChild(j).SetAsLastSibling();
            }
        }
        
        ResetButtonActionCardInDeck();
        countCardInDeck.text = cardsInDeck.Count + " / 8";
    }

    public void ResetCurrentDeck()
    {
        cardsInDeck.Clear();
    }

    IEnumerator ShowBigCart()
    {
        yield return timer;
        if (isTouchingACard)
        {
            bigCard.enabled = true;
        }
    }
}
