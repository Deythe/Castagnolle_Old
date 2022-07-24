using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BonesForDamage : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject targetUnit;
    [SerializeField] private int usingPhase = 3;
    private bool used;
    private int dmg=0;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (HaveABoneInGauge())
            {
                if (phase == 3)
                {
                    RoundManager.instance.StateRound = 6;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
                else if (phase == 6)
                {
                    dmg = 0;
                    targetUnit = EffectManager.instance.TargetUnit;
                    
                    for (int i = 0; i < DiceManager.instance.Gauge.Length; i++)
                    {
                        if (DiceManager.instance.Gauge[i].Equals(5))
                        {
                            DiceManager.instance.Gauge[i] = 0;
                            DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                                DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, 0);
                            dmg++;
                        }
                    }

                    view.RPC("RPC_Action", RpcTarget.AllViaServer, targetUnit.GetComponent<PhotonView>().ViewID,
                        dmg);
                    
                    DeckManager.instance.CheckUnitWithRessources();
                    EffectManager.instance.CancelSelection(1);
                    GetComponent<Monster>().p_model.layer = 6;
                    used = true;
                }
            }
            else
            {
                EffectManager.instance.CancelSelection(1);
                UiManager.instance.ShowTextFeedBackWithDelay(3);
                GetComponent<Monster>().p_model.layer = 6;
            }
        }
    }


    [PunRPC]
    private void RPC_Action(int unitID, int degat)
    {
        PlacementManager.instance.SearchMobWithID(unitID).p_atk-=(degat*3);
    }

    
    public bool HaveABoneInGauge()
    {
        foreach (var gaugedice in DiceManager.instance.Gauge)
        {
            if (gaugedice.Equals(5))
            {
                return true;
            }
        }

        return false;
    }
    
    public int GetPhaseActivation()
    {
        return usingPhase;
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
