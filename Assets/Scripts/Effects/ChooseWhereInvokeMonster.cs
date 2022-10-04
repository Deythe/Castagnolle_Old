using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ChooseWhereInvokeMonster : MonoBehaviour, IEffects
{
    [SerializeField] private GameObject cardInstance;
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases.Contains(phase))
            {
                PlacementManager.instance.SetGOPrefabsMonster(cardInstance.GetComponent<CardData>().p_prefabs);
                UiManager.instance.ShowingOffBigCard();
                GetComponent<MonstreData>().p_model.layer = 6;
            }
        }
    }

    public void TransferEffect(IEffects effectMother)
    {
        ChooseWhereInvokeMonster pivotCard = effectMother as ChooseWhereInvokeMonster;
        cardInstance = pivotCard.cardInstance;
        view = gameObject.GetPhotonView();
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
