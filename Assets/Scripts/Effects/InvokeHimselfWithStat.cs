using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InvokeHimselfWithStat : MonoBehaviour,IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject copyCardUnit;
    [SerializeField] private int usingPhase = 3;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (phase == usingPhase)
        {
            if (view.AmOwner)
            {
                Debug.Log("CacaKiPu");
                copyCardUnit = Instantiate(GetComponent<Monster>().Stats);
                copyCardUnit.GetComponent<CardData>().Atk = GetComponent<Monster>().Atk;
                PlacementManager.instance.SpecialInvocation = true;
                PlacementManager.instance.SetGOPrefabsMonster(copyCardUnit.GetComponent<CardData>().Prefabs);
                PlacementManager.instance.CurrentCardSelection = copyCardUnit.GetComponent<CardData>();
                UiManager.instance.ShowingOffBigCard();
                EffectManager.instance.CancelSelection(2);
                Destroy(copyCardUnit);
                PhotonNetwork.Destroy(gameObject);
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
