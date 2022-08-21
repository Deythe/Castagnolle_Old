using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Stun : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject targetUnit;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;

    private bool used;


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        /*
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                RoundManager.instance.p_roundState = 6;
                EffectManager.instance.CurrentUnit = gameObject;
            }
            else if (phase == 6)
            {
                view.RPC("RPC_Action", RpcTarget.AllViaServer, EffectManager.instance.p_unitTarget2.GetComponent<PhotonView>().ViewID);
                used = true;
                EffectManager.instance.CancelSelection(1);
                GetComponent<Monster>().p_model.layer = 6;
            }
        }

        if (phase == 2)
        {
            targetUnit.GetComponent<Monster>().p_isMovable = true;
          
            targetUnit.GetComponent<Monster>().p_attacked = false;
        }*/
    }
    
    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        targetUnit = PlacementManager.instance.SearchMobWithID(idTarget).gameObject;
        targetUnit.GetComponent<Monster>().p_isMovable = false;
        targetUnit.GetComponent<Monster>().p_attacked = true;
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
