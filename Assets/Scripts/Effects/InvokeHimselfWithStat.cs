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
    private int numberUnitOnBoard;

    
    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions[0] == condition)
            {
                numberUnitOnBoard = PlacementManager.instance.p_board.Count;
                PlacementManager.instance.SetGOPrefabsMonster(gameObject);
                UiManager.instance.ShowingOffBigCard();
                motherUnit = gameObject;
                GetComponent<MonstreData>().p_model.layer = 6;
                StartCoroutine(CoroutineOnCast());
            }
        }
    }

    IEnumerator CoroutineOnCast()
    {
        yield return new WaitUntil(()=> numberUnitOnBoard == PlacementManager.instance.p_board.Count+1);
        view.RPC("RPC_Action", RpcTarget.All, GetComponent<MonstreData>().p_id,
            motherUnit.GetComponent<MonstreData>().p_atk, motherUnit.GetComponent<MonstreData>().p_isMovable);
                    
        PhotonNetwork.Destroy(motherUnit);
        GetComponent<MonstreData>().p_model.layer = 6;
        motherUnit = null;
        PlacementManager.instance.p_haveAChampionOnBoard = true;
        used = true;
    }

    [PunRPC]
    private void RPC_Action(int id, int atk, bool mov)
    { 
        PlacementManager.instance.FindMobWithID(id).p_atk=atk;
        PlacementManager.instance.FindMobWithID(id).p_isMovable=mov;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = gameObject.GetPhotonView();
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
        motherUnit = null;
        numberUnitOnBoard = 0;
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
