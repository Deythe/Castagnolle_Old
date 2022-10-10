using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class KillWithHeroism : MonoBehaviour, IEffects
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
                view.RPC("RPC_Action", RpcTarget.AllViaServer,
                    EffectManager.instance.p_unitTarget1.GetComponent<PhotonView>().ViewID);
                
                GetComponent<MonstreData>().p_model.layer = 6;
                EffectManager.instance.CancelSelection();
                used = true;
            }
        }
    }

    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        PlacementManager.instance.FindMobWithID(idTarget).p_atk-= PlacementManager.instance.FindMobWithID(idTarget).p_atk;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = effectMother.GetView();
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
