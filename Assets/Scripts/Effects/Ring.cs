using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Ring : MonoBehaviour,IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    private bool used;


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (usingPhases[0].Equals(phase))
        {
            switch (BattlePhaseManager.instance.Result)
            {
                case >0:
                    if (view.AmOwner)
                    {
                        Debug.Log("RingAllié");
                        view.RPC("RPC_Ring", RpcTarget.AllViaServer,
                            BattlePhaseManager.instance.UnitSelected.GetComponent<PhotonView>().ViewID);
                    }

                    break;

                case <0:
                    if (!view.AmOwner)
                    {
                        Debug.Log("RingEnnemy");
                        view.RPC("RPC_Ring", RpcTarget.AllViaServer,
                            BattlePhaseManager.instance.TargetUnit.GetComponent<PhotonView>().ViewID);
                    }

                    break;
            }
        }
    }
    
    [PunRPC]
    private void RPC_Ring(int unitID)
    {
        Debug.Log("Rpc Ring");
        PlacementManager.instance.SearchMobWithID(unitID).p_atk++;
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
