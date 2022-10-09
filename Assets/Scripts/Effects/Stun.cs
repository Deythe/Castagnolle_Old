using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Stun : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;
    [SerializeField] private GameObject targetUnit;
    private void Awake()
    {
        isActivable = true;
    }
    
    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions[0] == condition)
            {
                targetUnit = EffectManager.instance.p_unitTarget1;
                view.RPC("RPC_Action", RpcTarget.AllViaServer,
                    EffectManager.instance.p_unitTarget1.GetComponent<PhotonView>().ViewID);
                used = true;
                EffectManager.instance.CancelSelection();
                GetComponent<MonstreData>().p_model.layer = 6;
            }
            else if (conditions[1] == condition)
            {
                if (targetUnit != null)
                {
                    targetUnit.GetComponent<MonstreData>().p_isMovable = true;
                    targetUnit.GetComponent<MonstreData>().p_attacked = false;
                } 
            }
        }
    }

    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        targetUnit = PlacementManager.instance.FindMobWithID(idTarget).gameObject;
        targetUnit.GetComponent<MonstreData>().p_isMovable = false;
        targetUnit.GetComponent<MonstreData>().p_attacked = true;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = gameObject.GetPhotonView();
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }

    public void ResetEffect()
    {
        used = false;
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

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
    
}
