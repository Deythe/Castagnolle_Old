using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvolve : MonoBehaviour, IPointerEnterHandler
{
    public GameObject unit;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        EffectManager.instance.AllieUnit = unit;
        EffectManager.instance.CurrentUnit.GetComponent<Monster>().ActivateEffects(8);
    }
}
