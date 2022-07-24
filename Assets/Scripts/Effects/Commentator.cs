using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Commentator : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 0;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                foreach (var unit in PlacementManager.instance.GetBoard())
                {
                    if (unit.monster.GetComponent<PhotonView>().AmOwner && !unit.monster.Equals(transform.gameObject))
                    {
                        InRange(unit.monster);
                    }
                }
                
                EffectManager.instance.CancelSelection(1);
                GetComponent<Monster>().p_model.layer = 6;
                used = true;
            }
        }
    }

    public void InRange(GameObject targetUnit)
    {
        foreach (var targetCenter in targetUnit.GetComponent<Monster>().GetCenters())
        {
            foreach (var center in transform.GetComponent<Monster>().GetCenters())
            {
                if (Vector3.Distance(center.position, targetCenter.position).Equals(1))
                {
                    view.RPC("RPC_Action", RpcTarget.AllViaServer, targetUnit.GetComponent<Monster>().p_id);
                    return;
                }
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action(int unitID)
    {
        PlacementManager.instance.SearchMobWithID(unitID).p_atk++;
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
