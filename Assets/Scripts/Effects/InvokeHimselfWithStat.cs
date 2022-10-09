using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InvokeHimselfWithStat : MonoBehaviour,IEffects
{
    public static GameObject motherUnit;
    
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
                PlacementManager.instance.SetGOPrefabsMonster(GetComponent<MonstreData>().p_stats.GetComponent<CardData>().p_prefabs);
                UiManager.instance.ShowingOffBigCard();
                motherUnit = gameObject;
                PlacementManager.instance.RemoveMonsterBoard(GetComponent<MonstreData>().p_id);
                EffectManager.instance.CancelSelection();
                UiManager.instance.p_textFeedBack.enabled = true;
                UiManager.instance.SetTextFeedBack(0);
                gameObject.SetActive(false);
            }
        }
        
        else if (condition == EffectManager.enumEffectConditionActivation.WhenThisUnitDie)
        {
            if (view.AmOwner)
            {
                if (motherUnit != null)
                {
                    view.RPC("RPC_Action", RpcTarget.All, GetComponent<MonstreData>().p_id,
                        motherUnit.GetComponent<MonstreData>().p_atk, motherUnit.GetComponent<MonstreData>().p_isMovable);
                    
                    PhotonNetwork.Destroy(motherUnit);
                    GetComponent<MonstreData>().p_model.layer = 6;
                    motherUnit = null;
                    used = true;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (motherUnit != null && !motherUnit.Equals(gameObject))
        {
            if (motherUnit.GetComponent<PhotonView>().AmOwner)
            {
                PlacementManager.instance.AddMonsterBoard(motherUnit);
                motherUnit.SetActive(true);
            }
        }
    }

    [PunRPC]
    private void RPC_Action(int id, int atk, bool mov)
    { 
        PlacementManager.instance.FindMobWithID(id).p_atk=atk;
        PlacementManager.instance.FindMobWithID(id).p_isMovable=mov;
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
    
    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
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

    public bool GetUsed()
    {
        return used;
    }
    
    public void ResetEffect()
    {
        used = false;
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
