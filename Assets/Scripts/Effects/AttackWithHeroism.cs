using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackWithHeroism : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    [SerializeField] private MonstreData targetUnit;

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            /*
            if (usingPhases[0].Equals(phase))
            {
                if (EffectManager.instance.CheckHeroism(transform))
                {
                    GetComponent<MonstreData>().p_attacked = false;
                    used = true;
                    EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                    GetComponent<MonstreData>().p_model.layer = 6;
                    GetComponent<MonstreData>().ChangeMeshRenderer(0);
                }
                else
                {
                    EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                    UiManager.instance.ShowTextFeedBackWithDelay(3);
                    GetComponent<MonstreData>().p_model.layer = 6;
                }
            }
            */
        }
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = effectMother.GetView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
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
