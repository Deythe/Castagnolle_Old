using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Evolve : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private List<CardData> listGameObjectUnit;
    private GameObject unitEvolved;

    private RectTransform cardListEvolve;

    private void Start()
    {
        cardListEvolve = UiManager.instance.p_carListToBeSelected;
    }


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases[0] == phase)
            {
                if (conditions.Count > 0)
                {
                    for (int i = 0; i < listGameObjectUnit.Count; i++)
                    {
                        cardListEvolve.GetChild(i).GetComponent<CardToBeSelected>().unit = listGameObjectUnit[i];
                        cardListEvolve.GetChild(i).GetComponent<Image>().sprite = listGameObjectUnit[i].p_fullCard;
                        cardListEvolve.GetChild(i).gameObject.SetActive(true);
                    }
                    conditions.Clear();
                    UiManager.instance.EnableDisableMenuNoChoice(true); 
                    GetComponent<MonstreData>().p_model.layer = 6;
                }
                else
                {
                    for (int i = cardListEvolve.childCount - 1; i >= 0; i--)
                    {
                        cardListEvolve.GetChild(i).gameObject.SetActive(false);
                    }

                    unitEvolved = PhotonNetwork.Instantiate(EffectManager.instance.p_unitTarget1.name, transform.position,
                        transform.rotation, 0);
                    
                    unitEvolved.GetComponent<MonstreData>().p_attacked = true;
                    EffectManager.instance.CancelSelection();
                    used = true;
                    GetComponent<MonstreData>().p_isChampion = false;
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

    public void TransferEffect(IEffects effectMother)
    {
        Evolve pivot = effectMother as Evolve;
        view = gameObject.GetPhotonView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
        
        listGameObjectUnit = new List<CardData>(pivot.listGameObjectUnit);
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
