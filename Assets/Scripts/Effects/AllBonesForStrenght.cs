using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AllBonesForStrenght : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    private bool used;
    private int check;


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases[0].Equals(phase))
            {
                check = 0;
                
                for (int i = 0; i < DiceManager.instance.p_diceGauge.Length; i++)
                {
                    if (DiceManager.instance.p_diceGauge[i].Equals(5))
                    {
                        DiceManager.instance.p_diceGauge[i] = 0;
                        DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.AllViaServer,
                            DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, 0);
                        check++;
                    }
                }
                
                view.RPC("RPC_Action", RpcTarget.All, check);
                check = 0; 
                
                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                GetComponent<Monster>().p_model.layer = 6;
                used = true;
            }
        }
    }

    [PunRPC]
    private void RPC_Action(int checks)
    {
        GetComponent<Monster>().p_atk+=(2*checks);
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
