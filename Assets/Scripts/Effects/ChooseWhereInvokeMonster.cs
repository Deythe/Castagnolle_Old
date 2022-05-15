using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ChooseWhereInvokeMonster : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject cardInstance;
    [SerializeField] private int usingPhase = 3;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (phase == 3)
        {
            if (view.AmOwner)
            {
                PhotonNetwork.Destroy(EffectManager.instance.AllieUnit);
                PlacementManager.instance.SpecialInvocation = true;
                PlacementManager.instance.SetGOPrefabsMonster(cardInstance.GetComponent<CardData>().Prefabs);
                PlacementManager.instance.CurrentCardSelection = cardInstance.GetComponent<CardData>();
                UiManager.instance.ShowingOffBigCard();
                PlacementManager.instance.InstantiateCurrent();
                EffectManager.instance.CancelSelection(2);
                used = true;
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
