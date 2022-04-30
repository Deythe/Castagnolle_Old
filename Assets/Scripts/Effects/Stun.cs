using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Stun : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    private GameObject targetUnit;
    private int usingPhase = 0;
    private bool used;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                RoundManager.instance.StateRound = 6;
            }
            else if (phase == 2)
            {
                if (targetUnit != null)
                {
                    targetUnit.GetComponent<Monster>().Status = 0;
                    targetUnit.GetComponent<Monster>().Attacked = false;
                }
            }
            else if (phase == 6)
            {
                targetUnit = EffectManager.instance.TargetUnit;
                view.RPC("RPC_Action", RpcTarget.All, targetUnit.GetComponent<PhotonView>().ViewID);
                used = true;
            }
        }

    }
    
    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        GameObject unit = PhotonView.Find(idTarget).gameObject; 
        unit.GetComponent<Monster>().Status = 1;
        unit.GetComponent<Monster>().Attacked = true;
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
