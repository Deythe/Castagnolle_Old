using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DestroyToInvoke : MonoBehaviour, IEffects
{
    [SerializeField] private GameObject prefabs;
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhase;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;

    private bool used;


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        /*
        if (view.AmOwner)
        {
            if (phase == 3)
            {
                RoundManager.instance.p_roundState = 7;
                EffectManager.instance.CurrentUnit = gameObject;
            }
            else if (phase == 7)
            {
                if (!EffectManager.instance.p_unitTarget1.Equals(gameObject))
                {
                    PhotonNetwork.Destroy(EffectManager.instance.p_unitTarget1);
                    PlacementManager.instance.SpecialInvocation = true;
                    PlacementManager.instance.SetGOPrefabsMonster(prefabs.GetComponent<CardData>().Prefabs);
                    UiManager.instance.ShowingOffBigCard();
                    EffectManager.instance.CancelSelection(2);
                    UiManager.instance.p_textFeedBack.enabled = true;
                    UiManager.instance.SetTextFeedBack(0);
                    UiManager.instance.EnableBorderStatus(68,168,254);
                    GetComponent<Monster>().p_model.layer = 6;
                    used = true;
                }
                else
                {
                    RoundManager.instance.p_roundState = 7;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
            }
        }*/
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
