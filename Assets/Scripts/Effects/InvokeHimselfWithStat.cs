using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InvokeHimselfWithStat : MonoBehaviour,IEffects
{
    public static GameObject motherUnit;
    
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 3;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (phase == usingPhase)
        {
            if (view.AmOwner)
            {
                PlacementManager.instance.SpecialInvocation = true;
                PlacementManager.instance.SetGOPrefabsMonster(GetComponent<Monster>().p_stats.GetComponent<CardData>().Prefabs);
                UiManager.instance.ShowingOffBigCard();
                motherUnit = gameObject;
                PlacementManager.instance.RemoveMonsterBoard(GetComponent<Monster>().p_id);
                EffectManager.instance.CancelSelection(2);
                UiManager.instance.p_textFeedBack.enabled = true;
                UiManager.instance.SetTextFeedBack(0);
                gameObject.SetActive(false);
            }
        }else if (phase == 0)
        {
            if (view.AmOwner)
            {
                if (motherUnit != null)
                {
                    view.RPC("RPC_Action", RpcTarget.All, GetComponent<Monster>().p_id,
                        motherUnit.GetComponent<Monster>().p_atk, motherUnit.GetComponent<Monster>().p_isMovable);
                    
                    PhotonNetwork.Destroy(motherUnit);
                    GetComponent<Monster>().p_model.layer = 6;
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
        PlacementManager.instance.SearchMobWithID(id).p_atk=atk;
        PlacementManager.instance.SearchMobWithID(id).p_isMovable=mov;
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
