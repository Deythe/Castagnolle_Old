using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvolve : MonoBehaviour, IPointerEnterHandler
{
    public GameObject unit;
    
    public GameObject Unit
    {
        set
        {
            unit = value;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        EffectManager.instance.TargetUnit = unit;
        EffectManager.instance.CurrentUnit.GetComponent<Monster>().ActivateEffects(7);
    }
}
