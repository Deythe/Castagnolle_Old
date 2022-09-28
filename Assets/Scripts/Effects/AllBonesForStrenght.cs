using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AllBonesForStrenght : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

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
                GetComponent<MonstreData>().p_model.layer = 6;
                used = true;
            }
        }
    }

    [PunRPC]
    private void RPC_Action(int checks)
    {
        GetComponent<MonstreData>().p_atk+=(2*checks);
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
