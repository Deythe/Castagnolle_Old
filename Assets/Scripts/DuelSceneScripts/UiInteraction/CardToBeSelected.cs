using UnityEngine;
using UnityEngine.EventSystems;

public class CardToBeSelected : MonoBehaviour, IPointerEnterHandler
{
    public CardData unit;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        EffectManager.instance.p_unitTarget1 = unit.p_prefabs;
        Debug.Log(EffectManager.instance.p_currentUnit);
        EffectManager.instance.CheckCondition();
    }
}
