using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackWithHeroism : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;

    [SerializeField] private MonstreData targetUnit;

    public void OnCast(EffectManager.enumEffectConditionActivation condition)
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
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectConditionActivation> GetConditions()
    {
        return conditions;
    }
    
    public List<EffectManager.enumActionEffect> GetActions()
    {
        return actions;
    }
    
    public bool GetIsActivable()
    {
        return isActivable;
    }

    public void SetIsActivable(bool b)
    {
        isActivable = b;
    }
    
    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
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
