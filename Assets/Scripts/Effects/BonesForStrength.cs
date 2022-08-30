using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BonesForStrength : MonoBehaviour, IEffects
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
            if (phase == 3)
            {
                if (HaveABoneInGauge())
                {
                    RoundManager.instance.p_roundState = 7;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
                else
                {
                    EffectManager.instance.CancelSelection(1);
                    UiManager.instance.ShowTextFeedBackWithDelay(3);
                }
            }
            else if (phase == 7)
            {
                Debug.Log("CACA");
                targetUnit = EffectManager.instance.p_unitTarget1;

                view.RPC("RPC_Action", RpcTarget.AllViaServer,
                    EffectManager.instance.p_unitTarget1.GetComponent<Monster>().p_id);

                for (int i = 0; i < DiceManager.instance.Gauge.Length; i++)
                {
                    if (DiceManager.instance.Gauge[i].Equals(5))
                    {
                        DiceManager.instance.Gauge[i] = 0;
                        DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                            DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, 0);
                        EffectManager.instance.CancelSelection(1);
                        used = true;
                        GetComponent<Monster>().p_model.layer = 6;
                        DeckManager.instance.CheckUnitWithRessources();
                        return;
                    }
                }
            }
        }
        */
    }

    public bool HaveABoneInGauge()
    {
        foreach (var gaugedice in DiceManager.instance.p_diceGauge)
        {
            if (gaugedice.Equals(5))
            {
                return true;
            }
        }

        return false;
    }
    

    [PunRPC]
    private void RPC_Action(int unitID)
    {
        PlacementManager.instance.SearchMobWithID(unitID).p_atk+=3;
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
