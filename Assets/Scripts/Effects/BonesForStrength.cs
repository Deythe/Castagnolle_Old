using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BonesForStrength : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject targetUnit;
    [SerializeField] private int usingPhase = 3;
    private bool used;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (HaveABoneInGauge())
            {
                if (phase == 3)
                {
                    RoundManager.instance.StateRound = 7;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
                else if (phase == 7)
                {
                    targetUnit = EffectManager.instance.AllieUnit;

                    view.RPC("RPC_Action", RpcTarget.AllViaServer,
                        EffectManager.instance.AllieUnit.GetComponent<Monster>().ID);

                    for (int i = 0; i < DiceManager.instance.Gauge.Length; i++)
                    {
                        if (DiceManager.instance.Gauge[i].Equals(5))
                        {
                            DiceManager.instance.Gauge[i] = 0;
                            DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                                DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, null);
                            EffectManager.instance.CancelSelection(1);
                            used = true;
                            return;
                        }
                    }

                }
            }
            else
            {
                Debug.Log("Pas de Bones");
                EffectManager.instance.CancelSelection(1);
            }
        }
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
    

    [PunRPC]
    private void RPC_Action(int unitID)
    {
        PlacementManager.instance.SearchMobWithID(unitID).Atk+=2;
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
