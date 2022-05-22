using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class KillWithHeroism : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject targetUnit;
    [SerializeField] private int usingPhase = 3;
    [SerializeField] private List<GameObject> mobNextTo;
    [SerializeField] private int heroism;
    private bool used;

    private void Start()
    {
        mobNextTo = new List<GameObject>();
    }

    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                if (EffectManager.instance.CheckHeroism(transform, mobNextTo, heroism))
                {
                    RoundManager.instance.StateRound = 6;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
                else
                {
                    EffectManager.instance.CancelSelection(1);
                }
            }
            else if (phase == 6)
            {
                view.RPC("RPC_Action", RpcTarget.AllViaServer,
                    EffectManager.instance.TargetUnit.GetComponent<PhotonView>().ViewID);

                used = true;
                EffectManager.instance.CancelSelection(1);
            }
        }
    }

    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        PlacementManager.instance.SearchMobWithID(idTarget).Atk-= PlacementManager.instance.SearchMobWithID(idTarget).Atk;
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
