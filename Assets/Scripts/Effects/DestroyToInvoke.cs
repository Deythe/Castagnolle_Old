using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DestroyToInvoke : MonoBehaviour, IEffects
{
    [SerializeField] private GameObject cardInstance;
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;

    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                if (!EffectManager.instance.p_unitTarget1.GetComponent<MonstreData>()
                    .HaveAnEffectThisPhase(EffectManager.enumEffectConditionActivation.WhenThisUnitDie))
                {
                    PhotonNetwork.Destroy(EffectManager.instance.p_unitTarget1);
                }
                else
                {
                    EffectManager.instance.p_unitTarget1.GetComponent<MonstreData>().TempoDeath();
                }
                PlacementManager.instance.SetGOPrefabsMonster(cardInstance.GetComponent<CardData>().p_prefabs);
                UiManager.instance.ShowingOffBigCard();
                GetComponent<MonstreData>().p_model.layer = 6;
                used = true;
            }
        }
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        DestroyToInvoke pivot = effectMother as DestroyToInvoke;
        view = gameObject.GetPhotonView();
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
        cardInstance = pivot.cardInstance;

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
    
    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
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
