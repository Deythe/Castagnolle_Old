using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardsMovable : MonoBehaviour, IPointerEnterHandler
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
                Page3.instance.p_bigCard.sprite = card;
                Page3.instance.p_cardMovableSprite.sprite = miniature;
                Page3.instance.p_isTouchingACard = true;
                Page3.instance.p_currentIndexCard = index;
            }
        }
    }
}
