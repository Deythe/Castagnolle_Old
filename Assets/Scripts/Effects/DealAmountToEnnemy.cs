using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DealAmountToEnnemy : MonoBehaviour,IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhase;
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
                view.RPC("RPC_Action", RpcTarget.AllViaServer, EffectManager.instance.p_unitTarget2.GetComponent<PhotonView>().ViewID, BattlePhaseManager.instance.TargetUnitAttack);
                used = true;
            }
        }*/
    }
    
    [PunRPC]
    private void RPC_Action(int idTarget, int damage)
    {
        PhotonView.Find(idTarget).gameObject.GetComponent<Monster>().p_atk -= damage;
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
