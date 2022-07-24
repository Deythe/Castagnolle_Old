using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardData : MonoBehaviour, IPointerEnterHandler
{
    public static bool isTouching;

    [SerializeField] private bool enabled;
    [SerializeField] private Sprite bigCard;
    [SerializeField] private GameObject prefabs;
    [SerializeField] private int atk;
    [SerializeField] private List<int> resources;
    [SerializeField] private bool isChampion;
    [SerializeField] private TMP_Text lifeCard;
    [SerializeField] private RectTransform resourceCard;
    [SerializeField] private Image image;

    private Color transparent = new Color(1, 1, 1, 0.1f);
    private ScrollRect scrollRectParent;
    private RectTransform rec;
    private float initialPositionY;

    public bool p_enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            if (enabled)
            {
                image.color = Color.white;
            }
            else
            {
                image.color = transparent;
            }
        }
    }
    public Sprite BigCard
    {
        get => bigCard;
    }
    public int Atk
    {
        get => atk;
        set
        {
            atk = value;
        }
    }
    
    public GameObject Prefabs
    {
        get => prefabs;
        set
        {
            prefabs = value;
        }
    }
    
    public bool IsChampion
    {
        get => isChampion;
        set
        {
            isChampion = value;
        }
    }
    
    public List<int> Ressources
    {
        get => resources;
        set
        {
            resources = value;
        }
    }
    
    private void Awake()
    {
        rec = GetComponent<RectTransform>();
        rec.localScale = new Vector3( GetComponentInParent<CanvasScaler>().referenceResolution.y / 1920f, GetComponentInParent<CanvasScaler>().referenceResolution.y / 1920f, GetComponentInParent<CanvasScaler>().referenceResolution.y / 1920f);
        scrollRectParent = GetComponentInParent<ScrollRect>();
    }

    private void OnEnable()
    {
        lifeCard.text = "" + atk;
        
        for (int i = 0; i < resources.Count; i++)
        {
            resourceCard.GetChild(i).GetComponent<Image>().sprite = DiceManager.instance.DiceListScriptable.symbolsList[resources[i]];
            resourceCard.GetChild(i).gameObject.SetActive(true);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && !isTouching)
            {
                UiManager.instance.AbleDeckCardTouch(transform);
                if (enabled)
                {
                    isTouching = true;
                    initialPositionY = transform.localPosition.y;
                    UiManager.instance.Card = gameObject;
                }
            }
        }
    }

    private void Update()
    {
        if (enabled)
        {
            if (isTouching && UiManager.instance.Card.Equals(gameObject) && Input.touchCount > 0)
            {
                rec.localPosition =
                    new Vector3(rec.localPosition.x, rec.localPosition.y + Input.GetTouch(0).deltaPosition.y,
                        rec.localPosition.z);

                if (Input.GetTouch(0).deltaPosition.y > 2)
                {
                    scrollRectParent.horizontal = false;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.touches[0].deltaPosition.x > 10 ||
                    Input.touches[0].deltaPosition.x < -10)
                {
                    ReInit();
                }
            }
        }
    }


    public void ReInit()
    {
        isTouching = false;
        UiManager.instance.ShowingOffBigCard();
        rec.localPosition = new Vector3(rec.localPosition.x, initialPositionY, rec.localPosition.z);
        scrollRectParent.horizontal = true;
        UiManager.instance.Card = null;
    }
}
