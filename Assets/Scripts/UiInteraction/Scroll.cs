using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scroll : MonoBehaviour, IPointerExitHandler
{
    public void OnPointerExit(PointerEventData eventData)
    {
        if (UiManager.instance.Card != null)
        {
            GetComponentInParent<ScrollRect>().horizontal = true;
            PlacementManager.instance.SetGOPrefabsMonster(UiManager.instance.Card.GetComponent<CardData>().Prefabs);
            PlacementManager.instance.CurrentCardSelection = UiManager.instance.Card.GetComponent<CardData>();
            UiManager.instance.ShowingOffBigCard();
            //PlacementManager.instance.InstantiateCurrent();
            RoundManager.instance.StateRound = 2;
            UiManager.instance.Card = null;
        }
    }
}
