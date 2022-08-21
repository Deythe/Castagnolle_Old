using System;
using System.Collections.Generic;
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
    
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhase;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    
    private RectTransform cardListEvolve;
    private GameObject unitPivot;
    private bool used;
    private int kill = 0;

    private void Start()
    {
        cardListEvolve = UiManager.instance.CarListChose;
    }


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        /*
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
                    
                    RoundManager.instance.p_roundState = 8;
                }
                else
                {
                    used = true;    
                }
                GetComponent<Monster>().p_model.layer = 6;
                
            }else if (phase == 8)
            {
                for (int i = cardListEvolve.childCount-1; i >= 0; i--)
                {
                    cardListEvolve.GetChild(i).gameObject.SetActive(false);
                }
                
                PhotonNetwork.Instantiate(EffectManager.instance.p_unitTarget1.name, transform.position,
                    transform.rotation, 0);
                
                EffectManager.instance.CancelSelection(1);
                used = true;

                GetComponent<Monster>().p_isChampion = false;
                PhotonNetwork.Destroy(gameObject);
            }
        }*/
    }

    List<EffectManager.enumEffectPhaseActivation> IEffects.GetPhaseActivation()
    {
        return usingPhase;
    }

    public List<EffectManager.enumConditionEffect> GetConditionsForActivation()
    {
        return conditions;
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
