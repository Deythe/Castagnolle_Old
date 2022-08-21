using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scroll : MonoBehaviour, IPointerExitHandler
{
    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (UiManager.instance.Card != null && Input.GetTouch(0).phase != TouchPhase.Ended)
            {
                scrollRect.horizontal = true;
                PlacementManager.instance.SetGOPrefabsMonster(UiManager.instance.Card.GetComponent<CardData>().Prefabs);
                PlacementManager.instance.CurrentCardSelection = UiManager.instance.Card.GetComponent<CardData>();
                UiManager.instance.ShowingOffBigCard();
                RoundManager.instance.p_roundState = RoundManager.enumRoundState.DragUnitPhase;
                UiManager.instance.Card.GetComponent<CardData>().ReInit();
                UiManager.instance.Card = null;
            }
        }
    }
}
