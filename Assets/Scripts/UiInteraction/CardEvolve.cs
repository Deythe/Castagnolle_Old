using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvolve : MonoBehaviour, IPointerEnterHandler
{
    public GameObject unit;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        EffectManager.instance.p_unitTarget1 = unit;
        EffectManager.instance.CurrentUnit.GetComponent<Monster>().ActivateEffects(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill);
    }
}
