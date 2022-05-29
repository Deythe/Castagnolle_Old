using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ChooseWhereInvokeMonster : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject cardInstance;
    [SerializeField] private int usingPhase = 0;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (phase == usingPhase)
        {
            if (view.AmOwner)
            {
                PlacementManager.instance.SpecialInvocation = true;
                PlacementManager.instance.SetGOPrefabsMonster(cardInstance.GetComponent<CardData>().Prefabs);
                UiManager.instance.ShowingOffBigCard();
                EffectManager.instance.CancelSelection(2);
                UiManager.instance.p_textFeedBack.enabled = true;
                UiManager.instance.SetTextFeedBack(0);
                UiManager.instance.EnableBorderStatus(68,168,254);
                GetComponent<Monster>().p_model.layer = 6;
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
