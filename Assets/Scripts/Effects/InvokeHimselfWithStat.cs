using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InvokeHimselfWithStat : MonoBehaviour,IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;
    [SerializeField] private int numberUnitOnBoard;
    private GameObject unitPivot;
    
    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                PlacementManager.instance.RemoveMonsterBoard(gameObject.GetComponent<MonstreData>().p_id);
                PlacementManager.instance.SetGOPrefabsMonster(GetComponent<MonstreData>().p_stats.p_prefabs);
                numberUnitOnBoard = PlacementManager.instance.p_board.Count;
                UiManager.instance.ShowingOffBigCard();
                GetComponent<MonstreData>().p_model.layer = 6;
                StartCoroutine(CoroutineOnCast());
            }
        }
    }

    IEnumerator CoroutineOnCast()
    {
        yield return new WaitUntil(()=> PlacementManager.instance.p_board.Count == numberUnitOnBoard+1);
        unitPivot = PlacementManager.instance.p_board[^1].monster;
        unitPivot.GetPhotonView().RPC("RPC_Action", RpcTarget.AllViaServer, unitPivot.GetComponent<MonstreData>().p_id,
            gameObject.GetComponent<MonstreData>().p_atk, gameObject.GetComponent<MonstreData>().p_isMovable);
        unitPivot.GetComponent<MonstreData>().p_model.layer = 6;
        unitPivot.GetComponent<MonstreData>().p_effect.SetUsed(true);
        PlacementManager.instance.p_haveAChampionOnBoard = true;
        PhotonNetwork.Destroy(gameObject);
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
    
    public void CancelEffect()
    {
        
    }
}
