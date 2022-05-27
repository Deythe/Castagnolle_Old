using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AllBonesForStrenght : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    private int check;
    private int usingPhase = 0;
    private bool used;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                check = 0;
                
                for (int i = 0; i < DiceManager.instance.Gauge.Length; i++)
                {
                    if (DiceManager.instance.Gauge[i].Equals(5))
                    {
                        DiceManager.instance.Gauge[i] = 0;
                        DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.AllViaServer,
                            DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, 0);
                        check++;
                    }
                }
                
                view.RPC("RPC_Action", RpcTarget.All, check);
                check = 0; 
                
                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CancelSelection(1);
                used = true;
            }
        }
    }

    [PunRPC]
    private void RPC_Action(int checks)
    {
        GetComponent<Monster>().Atk+=(2*checks);
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
