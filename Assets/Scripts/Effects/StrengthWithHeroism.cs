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
            if (phase==usingPhase)
            {
                if (EffectManager.instance.CheckHeroism(transform, mobNextTo, heroism))
                {
                    view.RPC("RPC_Action", RpcTarget.AllViaServer);
                    used = true;
                    EffectManager.instance.CancelSelection(1);
                } else
                {
                    EffectManager.instance.CancelSelection(1);
                    UiManager.instance.ShowTextFeedBackWithDelay(3);
                }
                
                GetComponent<Monster>().p_model.layer = 6;
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action()
    {
        GetComponent<Monster>().p_atk+=2;
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
