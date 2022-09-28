using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Stun : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    [SerializeField] private GameObject targetUnit;
    private void Awake()
    {
        isActivable = true;
    }
    
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases[0] == phase)
            {
                view.RPC("RPC_Action", RpcTarget.AllViaServer,
                    EffectManager.instance.p_unitTarget1.GetComponent<PhotonView>().ViewID);
                used = true;
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                GetComponent<MonstreData>().p_model.layer = 6;
            }
            else if (usingPhases[1] == phase)
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
        targetUnit = PlacementManager.instance.SearchMobWithID(idTarget).gameObject;
        targetUnit.GetComponent<MonstreData>().p_isMovable = false;
        targetUnit.GetComponent<MonstreData>().p_attacked = true;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = effectMother.GetView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
        isEffectAuto = effectMother.GetIsEffectAuto();
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
