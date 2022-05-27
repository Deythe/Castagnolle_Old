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
                PlacementManager.instance.SetGOPrefabsMonster(GetComponent<Monster>().Stats.GetComponent<CardData>().Prefabs);
                UiManager.instance.ShowingOffBigCard();
                motherUnit = gameObject;
                
                PlacementManager.instance.RemoveMonsterBoard(GetComponent<Monster>().ID);
                EffectManager.instance.CancelSelection(2);
                gameObject.SetActive(false);
            }
        }else if (phase == 0)
        {
            if (view.AmOwner)
            {
                if (motherUnit != null)
                {
                    view.RPC("RPC_Action", RpcTarget.AllViaServer, GetComponent<Monster>().ID,
                        motherUnit.GetComponent<Monster>().Atk);
                    
                    PhotonNetwork.Destroy(motherUnit);
                    motherUnit = null;
                    used = true;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (motherUnit != null)
        {
            if (motherUnit.GetComponent<PhotonView>().AmOwner)
            {
                PlacementManager.instance.AddMonsterBoard(motherUnit);
                motherUnit.SetActive(true);
            }
        }
    }

    [PunRPC]
    private void RPC_Action(int id, int atk)
    { 
        PlacementManager.instance.SearchMobWithID(id).Atk=atk;
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
