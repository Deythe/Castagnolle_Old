using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DealAmountToEnnemy : MonoBehaviour,IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 1;
    private bool used;

    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                RoundManager.instance.StateRound = 6;
                EffectManager.instance.CurrentUnit = gameObject;
            }
            else if (phase == 6)
            {
                view.RPC("RPC_Action", RpcTarget.AllViaServer, EffectManager.instance.TargetUnit.GetComponent<PhotonView>().ViewID, BattlePhaseManager.instance.TargetUnitAttack);
                used = true;
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action(int idTarget, int damage)
    {
        PhotonView.Find(idTarget).gameObject.GetComponent<Monster>().Atk -= damage;
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
