using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scroll : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public void OnPointerExit(PointerEventData eventData)
    {
        if (UiManager.instance.GetCard() != null)
        {
            GetComponent<ScrollRect>().horizontal = true;
            PlacementManager.instance.SetGOPrefabsMonster(UiManager.instance.GetCard().GetComponent<CardData>().GetStat().prefabs);
            UiManager.instance.ShowingOffBigCard();
            PlacementManager.instance.InstantiateCurrent();
            RoundManager.instance.SetStateRound(2);
            UiManager.instance.SetCard(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }
}
