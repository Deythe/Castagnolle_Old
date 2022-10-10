using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AddAtkForTouchingAllyUnit : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;

    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                foreach (var unit in PlacementManager.instance.p_board)
                {
                    if (unit.monster.GetComponent<PhotonView>().AmOwner && !unit.monster.Equals(transform.gameObject))
                    {
                        InRange(unit.monster);
                    }
                }
                
                EffectManager.instance.CancelSelection();
                GetComponent<MonstreData>().p_model.layer = 6;
                used = true;
            }
        }
        
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
        PlacementManager.instance.FindMobWithID(unitID).p_atk++;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = gameObject.GetPhotonView();
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectConditionActivation> GetConditions()
    {
        return conditions;
    }
    
    public List<EffectManager.enumActionEffect> GetActions()
    {
        return actions;
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
    
    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
    }

    public void SetUsed(bool b)
    {
        used = b;
    }
    
    public void ResetEffect()
    {
        used = false;
    }

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
    
    public void CancelEffect()
    {
        
    }
}
