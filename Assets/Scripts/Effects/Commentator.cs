using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Commentator : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        /*
        if (view.AmOwner)
        {
            if (usingPhase[0].Equals(phase))
            {
                foreach (var unit in PlacementManager.instance.p_board)
                {
                    if (unit.monster.GetComponent<PhotonView>().AmOwner && !unit.monster.Equals(transform.gameObject))
                    {
                        InRange(unit.monster);
                    }
                }
                
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                GetComponent<MonstreData>().p_model.layer = 6;
                used = true;
            }
        }
        */
    }

    public void InRange(GameObject targetUnit)
    {
        foreach (var targetCenter in targetUnit.GetComponent<MonstreData>().GetCenters())
        {
            foreach (var center in transform.GetComponent<MonstreData>().GetCenters())
            {
                if (Vector3.Distance(center.position, targetCenter.position).Equals(1))
                {
                    view.RPC("RPC_Action", RpcTarget.AllViaServer, targetUnit.GetComponent<MonstreData>().p_id);
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
    
    public void TransferEffect(IEffects effectMother)
    {
        view = effectMother.GetView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectPhaseActivation> GetUsingPhases()
    {
        return usingPhases;
    }
    
    public List<EffectManager.enumConditionEffect> GetConditions()
    {
        return conditions;
    }
    
    public bool GetIsActivable()
    {
        return isActivable;
    }

    public void SetIsActivable(bool b)
    {
        isActivable = b;
    }

    public bool GetUsed()
    {
        return used;
    }

    public void SetUsed(bool b)
    {
        used = b;
    }

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
}
