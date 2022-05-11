using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DestroyToInvoke : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    private int usingPhase = 3;
    private bool used;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == 5)
            {
                RoundManager.instance.StateRound = 7;
                EffectManager.instance.CurrentUnit = gameObject;
            }
            else if (phase == 7)
            {
                Debug.Log("Prout");
                used = true;
                EffectManager.instance.CancelSelection();
            }
        }
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
