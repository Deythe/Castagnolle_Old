using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StrengthWithHeroism : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private List<GameObject> mobNextTo;
    [SerializeField] private int heroism;
    private bool used;


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases[0].Equals(0))
            {
                if (EffectManager.instance.CheckHeroism(transform, mobNextTo, heroism))
                {
                    view.RPC("RPC_Action", RpcTarget.AllViaServer);
                    used = true;
                    EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                } else
                {
                    EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                    UiManager.instance.ShowTextFeedBackWithDelay(3);
                }
                
                GetComponent<Monster>().p_model.layer = 6;
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action()
    {
        GetComponent<Monster>().p_atk+=2;
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
