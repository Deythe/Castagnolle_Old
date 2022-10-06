using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackXTime : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private int xAttack, xMaxAttack;

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases.Contains(phase))
            {
                if (xAttack <= xMaxAttack)
                {
                    transform.GetComponent<MonstreData>().p_attacked = false;
                }
                else
                {
                    used = true;
                    xAttack=0;
                    GetComponent<MonstreData>().p_model.layer = 6;
                }

                xAttack++;
                EffectManager.instance.CancelSelection();
            }
        }
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        AttackXTime pivot = effectMother as AttackXTime;
        view = gameObject.GetPhotonView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
        xAttack = pivot.xAttack;
        xMaxAttack = pivot.xMaxAttack;
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectPhaseActivation> GetUsingPhases()
    {
        return usingPhases;
    }
    
    public List<EffectManager.enumConditionEffect> GetConditions()
    {
        return conditions;
    }
    
    public bool GetIsActivable()
    {
        return isActivable;
    }

    public void SetIsActivable(bool b)
    {
        isActivable = b;
    }

    public bool GetUsed()
    {
        return used;
    }

    public void SetUsed(bool b)
    {
        used = b;
    }

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
    
}
