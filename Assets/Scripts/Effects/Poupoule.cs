using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Poupoule : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject targetUnit;
    [SerializeField] private int usingPhase = 5;
    private bool used;


    public void OnCast(int phase)
    {
        if (phase == usingPhase)
        {
            if (!view.AmOwner)
            {
                if (BattlePhaseManager.instance.TargetUnit.Equals(gameObject))
                {
                    if (BattlePhaseManager.instance.Result > 0)
                    {
                        Debug.Log("Prout");
                    }
                }
            }
            else
            {
                if (BattlePhaseManager.instance.UnitSelected.Equals(gameObject))
                {
                    if (BattlePhaseManager.instance.Result < 0)
                    {
                        Debug.Log("CacaquiPu");
                    }
                }
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        PlacementManager.instance.SearchMobWithID(idTarget).Atk++;
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
