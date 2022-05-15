using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Ring : MonoBehaviour,IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 5;
    private bool used;


    public void OnCast(int phase)
    {
        if (usingPhase==phase)
        {
            switch (BattlePhaseManager.instance.Result)
            {
                case >0:
                    if (view.AmOwner)
                    {
                        Debug.Log("RingAllié");
                        view.RPC("RPC_Ring", RpcTarget.AllViaServer,
                            BattlePhaseManager.instance.UnitSelected.GetComponent<PhotonView>().ViewID);
                    }

                    break;

                case <0:
                    if (!view.AmOwner)
                    {
                        Debug.Log("RingEnnemy");
                        view.RPC("RPC_Ring", RpcTarget.AllViaServer,
                            BattlePhaseManager.instance.TargetUnit.GetComponent<PhotonView>().ViewID);
                    }

                    break;
            }
        }
    }
    
    [PunRPC]
    private void RPC_Ring(int unitID)
    {
        Debug.Log("Rpc Ring");
        PlacementManager.instance.SearchMobWithID(unitID).Atk++;
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
