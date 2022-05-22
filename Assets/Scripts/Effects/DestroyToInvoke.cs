using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DestroyToInvoke : MonoBehaviour, IEffects
{
    [SerializeField] private GameObject prefabs;
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 3;
    private bool used;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == 3)
            {
                RoundManager.instance.StateRound = 7;
                EffectManager.instance.CurrentUnit = gameObject;
            }
            else if (phase == 7)
            {
                if (!EffectManager.instance.AllieUnit.Equals(gameObject))
                {
                    PhotonNetwork.Destroy(EffectManager.instance.AllieUnit);
                    PlacementManager.instance.SpecialInvocation = true;
                    PlacementManager.instance.SetGOPrefabsMonster(prefabs.GetComponent<CardData>().Prefabs);
                    PlacementManager.instance.CurrentCardSelection = prefabs.GetComponent<CardData>();
                    UiManager.instance.ShowingOffBigCard();
                    EffectManager.instance.CancelSelection(2);
                    used = true;
                }
                else
                {
                    RoundManager.instance.StateRound = 7;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
            }
        }
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
