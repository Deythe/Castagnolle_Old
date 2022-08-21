using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class KillWithHeroism : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject targetUnit;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private List<GameObject> mobNextTo;
    [SerializeField] private int heroism;
    private bool used;

    private void Start()
    {
        mobNextTo = new List<GameObject>();
    }

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        /*
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                if (EffectManager.instance.CheckHeroism(transform, mobNextTo, heroism))
                {
                    RoundManager.instance.p_roundState = 6;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
                else
                {
                    EffectManager.instance.CancelSelection(1);
                    UiManager.instance.ShowTextFeedBackWithDelay(3);
                    GetComponent<Monster>().p_model.layer = 6;
                }
                
            }
            else if (phase == 6)
            {
                view.RPC("RPC_Action", RpcTarget.AllViaServer,
                    EffectManager.instance.p_unitTarget2.GetComponent<PhotonView>().ViewID);

                used = true;
                GetComponent<Monster>().p_model.layer = 6;
                EffectManager.instance.CancelSelection(1);
            }
        }*/
    }

    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        PlacementManager.instance.SearchMobWithID(idTarget).p_atk-= PlacementManager.instance.SearchMobWithID(idTarget).p_atk;
    }

    List<EffectManager.enumEffectPhaseActivation> IEffects.GetPhaseActivation()
    {
        return usingPhases;
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
