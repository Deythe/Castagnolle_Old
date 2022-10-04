using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EvolveByKilling : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    
    [SerializeField] private CardData card;
    [SerializeField] private GameObject blankCard;
    
    [SerializeField] private Sprite evolve1;
    [SerializeField] private Sprite evolve2;
    
    [SerializeField] private GameObject unit1;
    [SerializeField] private GameObject unit2;
    
    private RectTransform cardListEvolve;
    private GameObject unitPivot;
    private int maxKill, currentKill;

    private void Start()
    {
        currentKill = 0;
        maxKill = 2;
        cardListEvolve = UiManager.instance.CarListChose;
    }


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases[0] == phase)
            {
                if (currentKill != maxKill)
                {
                    currentKill++;
                }
                else
                {
                    if (cardListEvolve.childCount == 0)
                    {
                        Debug.Log("Effet Rey Mysteriorc");
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
                        for (int i = cardListEvolve.childCount - 1; i >= 0; i--)
                        {
                            cardListEvolve.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                }

                GetComponent<MonstreData>().p_model.layer = 6;
            }
            
            else if (usingPhases[1] == phase)
            {
                for (int i = cardListEvolve.childCount-1; i >= 0; i--)
                {
                    cardListEvolve.GetChild(i).gameObject.SetActive(false);
                }
                
                PhotonNetwork.Instantiate(EffectManager.instance.p_unitTarget1.name, transform.position,
                    transform.rotation, 0);
                
                EffectManager.instance.CancelSelection();
                used = true;
                GetComponent<MonstreData>().p_isChampion = false;
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        EvolveByKilling pivot = effectMother as EvolveByKilling;
        view = effectMother.GetView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
        
        unit1 = pivot.unit1;
        unit2 = pivot.unit2;
        blankCard = pivot.blankCard;
        evolve1 = pivot.evolve1;
        evolve2 = pivot.evolve2;
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectPhaseActivation> GetUsingPhases()
    {
        return usingPhases;
    }
    
    public List<EffectManager.enumConditionEffect> GetConditions()
    {
        return conditions;
    }
    
    public bool GetIsActivable()
    {
        return isActivable;
    }

    public void SetIsActivable(bool b)
    {
        isActivable = b;
    }

    public bool GetUsed()
    {
        return used;
    }

    public void SetUsed(bool b)
    {
        used = b;
    }

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
}
