using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardBuildingDeck : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private int index;
    [SerializeField] private Sprite miniature;
    [SerializeField] private Sprite card;

    private void Start()
    {
        card = GetComponent<Image>().sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                CardBuildingManager.instance.p_bigCard.sprite = card;
                CardBuildingManager.instance.p_cardMovableSprite.sprite = miniature;
                CardBuildingManager.instance.p_isTouchingACard = true;
                CardBuildingManager.instance.p_currentIndexCard = index;
            }
        }
    }
}
