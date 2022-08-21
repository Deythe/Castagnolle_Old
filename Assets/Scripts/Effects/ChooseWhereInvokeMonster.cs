using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ChooseWhereInvokeMonster : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject cardInstance;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhase;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    private bool used;
    
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (usingPhase[0].Equals(phase))
        {
            if (view.AmOwner)
            {
                PlacementManager.instance.SpecialInvocation = true;
                PlacementManager.instance.SetGOPrefabsMonster(cardInstance.GetComponent<CardData>().Prefabs);
                UiManager.instance.ShowingOffBigCard();
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DragUnitPhase);
                UiManager.instance.p_textFeedBack.enabled = true;
                UiManager.instance.SetTextFeedBack(0);
                UiManager.instance.EnableBorderStatus(68,168,254);
                GetComponent<Monster>().p_model.layer = 6;
            }
        }
    }

    List<EffectManager.enumEffectPhaseActivation> IEffects.GetPhaseActivation()
    {
        return usingPhase;
    }

    public List<EffectManager.enumConditionEffect> GetConditionsForActivation()
    {
        return conditions;
    }

    public bool GetUsed()
    {
        return used;
    }
    
    public void SetUsed(bool b)
    {
        used = b;
    }
}
