using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Commentator : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhase;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;

    private bool used;
    
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhase[0].Equals(phase))
            {
                foreach (var unit in PlacementManager.instance.GetBoard())
                {
                    if (unit.monster.GetComponent<PhotonView>().AmOwner && !unit.monster.Equals(transform.gameObject))
                    {
                        InRange(unit.monster);
                    }
                }
                
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                GetComponent<Monster>().p_model.layer = 6;
                used = true;
            }
        }
    }

    public void InRange(GameObject targetUnit)
    {
        foreach (var targetCenter in targetUnit.GetComponent<Monster>().GetCenters())
        {
            foreach (var center in transform.GetComponent<Monster>().GetCenters())
            {
                if (Vector3.Distance(center.position, targetCenter.position).Equals(1))
                {
                    view.RPC("RPC_Action", RpcTarget.AllViaServer, targetUnit.GetComponent<Monster>().p_id);
                    return;
                }
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action(int unitID)
    {
        PlacementManager.instance.SearchMobWithID(unitID).p_atk++;
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
