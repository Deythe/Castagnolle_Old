using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Evolve : MonoBehaviour, IEffects
{
    [SerializeField] private CardData card;
        
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject blankCard;
    
    [SerializeField] private Sprite evolve1;
    [SerializeField] private Sprite evolve2;
    
    [SerializeField] private GameObject unit1;
    [SerializeField] private GameObject unit2;
    
    private RectTransform cardListEvolve;
    private GameObject unitPivot;
    [SerializeField] private int usingPhase = 1;
    private bool used;
    private int kill = 0;

    private void Start()
    {
        cardListEvolve = UiManager.instance.CarListChose;
    }


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (usingPhase == phase)
            {
                kill++;
                Debug.Log("Kill++");
                if (kill == 2)
                {
                    if (cardListEvolve.childCount==0)
                    {
                        Debug.Log("Effet Rey Mysteriorc");
                        EffectManager.instance.CurrentUnit = gameObject;
                        
                        unitPivot = Instantiate(blankCard, cardListEvolve);
                        unitPivot.GetComponent<Image>().sprite = evolve1;
                        unitPivot.GetComponent<CardEvolve>().unit = unit1;

                        unitPivot = Instantiate(blankCard, cardListEvolve);
                        unitPivot.GetComponent<Image>().sprite = evolve2;
                        unitPivot.GetComponent<CardEvolve>().unit = unit2;

                        unitPivot = null;
                    }
                    else
                    {
                        for (int i = cardListEvolve.childCount-1; i >= 0; i--)
                        {
                            cardListEvolve.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                    
                    RoundManager.instance.StateRound = 8;
                }
                else
                {
                    used = true;    
                }
                
            }else if (phase == 8)
            {
                used = true;
                for (int i = cardListEvolve.childCount-1; i >= 0; i--)
                {
                    cardListEvolve.GetChild(i).gameObject.SetActive(false);
                }
                
                PlacementManager.instance.SpecialInvocation = true;
                PlacementManager.instance.SetGOPrefabsMonster(EffectManager.instance.AllieUnit);
                UiManager.instance.ShowingOffBigCard();
                EffectManager.instance.CancelSelection(2);
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
