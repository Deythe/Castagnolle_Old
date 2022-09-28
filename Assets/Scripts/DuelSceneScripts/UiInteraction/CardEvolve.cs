using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvolve : MonoBehaviour, IPointerEnterHandler
{
    public GameObject unit;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        EffectManager.instance.p_unitTarget1 = unit;
        EffectManager.instance.p_currentUnit.GetComponent<MonstreData>().ActivateEffects(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill);
    }
}
