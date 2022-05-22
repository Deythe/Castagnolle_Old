using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MoveAnUnit : MonoBehaviour,IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject cardUnit;
    [SerializeField] private GameObject copyCardUnit;
    [SerializeField] private int usingPhase = 0;
    private bool used;

    public void OnCast(int phase)
    {
        if (usingPhase == phase)
        {
            if (view.AmOwner)
            {
                if (phase == usingPhase)
                {
                    RoundManager.instance.StateRound = 7;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
                else if (phase == 7)
                {
                    
                    copyCardUnit = Instantiate(EffectManager.instance.AllieUnit.GetComponent<Monster>().Stats);
                    copyCardUnit.GetComponent<CardData>().Atk = EffectManager.instance.AllieUnit.GetComponent<Monster>().GetComponent<Monster>().Atk;
                    
                    PlacementManager.instance.SpecialInvocation = true;
                    PlacementManager.instance.SetGOPrefabsMonster(copyCardUnit.GetComponent<CardData>().Prefabs);
                    PlacementManager.instance.CurrentCardSelection = copyCardUnit.GetComponent<CardData>();
                    
                    UiManager.instance.ShowingOffBigCard();
                    EffectManager.instance.CancelSelection(2);
                    PhotonNetwork.Destroy(gameObject);
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
