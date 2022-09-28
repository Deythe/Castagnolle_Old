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
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    private void Awake()
    {
        isActivable = true;
    }

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        Debug.Log("Ring passe effet");
        if (usingPhases.Contains(phase))
        {
            switch (CastagneManager.instance.p_result)
            {
                case >0:
                    if (view.AmOwner)
                    {
                        Debug.Log("RingAllié");
                        view.RPC("RPC_Ring", RpcTarget.AllViaServer,
                            CastagneManager.instance.p_unitSelected.GetComponent<PhotonView>().ViewID);
                    }

                    break;

                case <0:
                    if (!view.AmOwner)
                    {
                        Debug.Log("RingEnnemy");
                        view.RPC("RPC_Ring", RpcTarget.AllViaServer,
                            CastagneManager.instance.p_unitTarget.GetComponent<PhotonView>().ViewID);
                    }

                    break;
            }
            EffectManager.instance.CancelSelection(RoundManager.enumRoundState.CastagnePhase);
        }
    }
    
    [PunRPC]
    private void RPC_Ring(int unitID)
    {
        Debug.Log("Rpc Ring");
        PlacementManager.instance.SearchMobWithID(unitID).p_atk++;
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
