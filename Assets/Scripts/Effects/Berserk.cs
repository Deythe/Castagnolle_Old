using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Berserk : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    private int usingPhase = 1;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (usingPhase == phase)
        {
            if (view.AmOwner)
            {
                transform.GetComponent<Monster>().Attacked = false;
                used = true;
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
