using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserk : MonoBehaviour, IEffects
{
    private int usingPhase = 1;
    private bool used;
    
    public void OnCast()
    {
        transform.GetComponent<Monster>().SetAttacked(false);
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
