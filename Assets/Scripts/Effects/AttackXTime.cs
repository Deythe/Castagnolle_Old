using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackXTime : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private int numberAttack;
    private int currentAttack;
    private bool used;

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (usingPhases[0].Equals(phase))
        {
            if (view.AmOwner)
            {
                if (currentAttack < numberAttack)
                {
                    transform.GetComponent<Monster>().p_attacked = false;
                }
                else
                {
                    used = true;
                    GetComponent<Monster>().p_model.layer = 6;
                }
            }
        }
    }
    
    List<EffectManager.enumEffectPhaseActivation> IEffects.GetPhaseActivation()
    {
        return usingPhases;
    }

    public List<EffectManager.enumConditionEffect> GetConditionsForActivation()
    {
        return conditions;
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
