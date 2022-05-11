using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AddAtkForNextUnitDead : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    private int usingPhase = 5;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
           
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
