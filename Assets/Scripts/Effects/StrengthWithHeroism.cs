using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StrengthWithHeroism : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 3;
    [SerializeField] private List<GameObject> mobNextTo;
    [SerializeField] private int heroism;
    private bool used;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (EffectManager.instance.CheckHeroism(transform, mobNextTo, heroism) && phase==usingPhase)
            {
                view.RPC("RPC_Action", RpcTarget.AllViaServer, GetComponent<Monster>().ID);
                used = true;
                EffectManager.instance.CancelSelection(1);
            }
            else
            {
                EffectManager.instance.CancelSelection(1);
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        PlacementManager.instance.SearchMobWithID(idTarget).Atk+=2;
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