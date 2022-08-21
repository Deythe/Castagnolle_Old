using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    
    public enum enumEffectPhaseActivation
    {
        WhenThisUnitIsInvoke, WhenThisUnitKill, WhenAUnitDie, WhenItsDrawPhase, WhenThisUnitDie
    }
    
    public enum enumConditionEffect
    {
        SelectAllyUnit, SelectEnemyUnit, Heroism, HaveABoneInGauge, SelectACard
    }
    
    [SerializeField] private PhotonView view;
    [SerializeField] private List<GameObject> effectsList;
    [SerializeField] private List<GameObject> instantiateEffect;

    [SerializeField] private List<enumConditionEffect> copyCurentUnitCondition;
    
    [SerializeField] private GameObject currentUnit;
    [SerializeField] private GameObject unitTarget1;
    [SerializeField] private GameObject unitTarget2;
    [SerializeField] private Transform pooler;
    
    private bool unitSelected;
    private RaycastHit hit;
    private int numberIdAll = 0;
    private int i, j;

    public PhotonView View
    {
        get => view;
    }

    public GameObject CurrentUnit
    {
        get => currentUnit;
        set
        {
            currentUnit = value;
        }
    }
    
    public GameObject p_unitTarget1
    {
        get => unitTarget1;
        set
        {
            unitTarget1 = value;
        }
    }

    public GameObject p_unitTarget2
    {
        get => unitTarget2;
        set
        {
            unitTarget2 = value;
        }
    }

    public void InstantiateAllEffect()
    {
        for (i = 0; i < effectsList.Count ; i++)
        {
            instantiateEffect.Add(Instantiate(effectsList[i],Vector3.zero, PlayerSetup.instance.transform.rotation, pooler));
            instantiateEffect[i].SetActive(false);
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (RoundManager.instance != null)
        {
            SelectTarget();
        }
    }

    void SelectTarget()
    {
        if (RoundManager.instance.p_roundState == RoundManager.enumRoundState.DrawPhase || RoundManager.instance.p_roundState == RoundManager.enumRoundState.EffectPhase && PlacementManager.instance.GetPrefabUnit() == null)
        {
            if (Input.touchCount > 0)
            {
                switch (Input.GetTouch(0).phase)
                {
                    case TouchPhase.Began:
                        Physics.Raycast(PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position), out hit);
                        if (hit.collider != null)
                        {
                            if (hit.collider.GetComponent<Monster>() != null)
                            {
                                switch (RoundManager.instance.p_roundState)
                                {
                                    case RoundManager.enumRoundState.DrawPhase:
                                        if (hit.collider.GetComponent<PhotonView>().AmOwner)
                                        {
                                            if (hit.collider.GetComponent<Monster>()
                                                .HaveAnEffectThisPhase(enumEffectPhaseActivation.WhenItsDrawPhase))
                                            {
                                                currentUnit = hit.collider.gameObject;
                                                UiManager.instance.EnableDisableMenuNoChoice(true);
                                                UiManager.instance.EnableDisableMenuYesChoice(true);
                                            }
                                        }
                                        break;
                                    
                                    case RoundManager.enumRoundState.EffectPhase:
                                        switch (copyCurentUnitCondition[0])
                                        {
                                            case enumConditionEffect.SelectAllyUnit:
                                                if (hit.collider.GetComponent<PhotonView>().AmOwner)
                                                {
                                                    if (!hit.collider.gameObject.Equals(currentUnit))
                                                    {
                                                        if (unitTarget1 == null)
                                                        {
                                                            unitTarget1 = hit.collider.gameObject;
                                                        }
                                                        else
                                                        {
                                                            unitTarget2 = hit.collider.gameObject;
                                                        }

                                                        copyCurentUnitCondition.RemoveAt(0);
                                                        CheckCondition();
                                                    }
                                                }
                                                break;
                                            
                                            case enumConditionEffect.SelectEnemyUnit:
                                                if (!hit.collider.GetComponent<PhotonView>().AmOwner)
                                                {
                                                    if (unitTarget1 == null)
                                                    {
                                                        unitTarget1 = hit.collider.gameObject;
                                                    }
                                                    else
                                                    {
                                                        unitTarget2 = hit.collider.gameObject;
                                                    }

                                                    copyCurentUnitCondition.RemoveAt(0);
                                                    CheckCondition();
                                                }
                                                break;

                                        }
                                        break;
                                }
                            }
                        }
                        
                        break;
                }
            }
        }
    }

    private void CheckCondition()
    {
        switch (copyCurentUnitCondition[0])
        {
            case enumConditionEffect.SelectAllyUnit:
                UiManager.instance.SetTextFeedBack(0);
                UiManager.instance.p_textFeedBack.enabled = true;
                break;
            case enumConditionEffect.SelectEnemyUnit:
                UiManager.instance.SetTextFeedBack(0);
                UiManager.instance.p_textFeedBack.enabled = true;
                break;
        }
        
        if (copyCurentUnitCondition.Count == 0)
        {
            UiManager.instance.EnableDisableMenuYesChoice(true);
        }
        else
        {
            UiManager.instance.EnableDisableMenuYesChoice(false); 
        }
    }
    
    public void ActiveEffect()
    {
        currentUnit.GetComponent<Monster>().ActivateEffects(enumEffectPhaseActivation.WhenItsDrawPhase);
    }

    public void UnitSelected()
    {
        RoundManager.instance.p_roundState = RoundManager.enumRoundState.EffectPhase;
        copyCurentUnitCondition =
            new List<enumConditionEffect>(currentUnit.GetComponent<Monster>()
                .GetConditionsListFromEffect(enumEffectPhaseActivation.WhenItsDrawPhase));

        if (copyCurentUnitCondition.Count.Equals(0))
        {
            ActiveEffect();
        }
        else
        {
            CheckCondition();
        }
    }

    public void CancelSelection(RoundManager.enumRoundState state)
    {
        ClearUnits();
        RoundManager.instance.p_roundState = state;
    }

    public void ClearUnits()
    {
        copyCurentUnitCondition = null;
        UiManager.instance.EnableDisableMenuNoChoice(false);
        UiManager.instance.EnableDisableMenuYesChoice(false);
        UiManager.instance.p_textFeedBack.enabled = false;
        unitTarget1 = null;
        unitTarget2 = null;
        currentUnit = null;
    }
    
    public bool CheckHeroism(Transform go, List<GameObject> mobNextTo, int numberCheck)
    {
        for (i = 0; i < numberCheck; i++)
        {
            for (j = 0; j < PlacementManager.instance.GetBoard().Count; j++)
            {
                foreach (var unitAlly in go.GetComponent<Monster>().GetCenters())
                {
                    if (!PlacementManager.instance.GetBoard()[j].monster.GetComponent<PhotonView>().AmOwner)
                    {
                        foreach (var center in PlacementManager.instance.GetBoard()[j].emplacement)
                        {
                            if (Vector2.Distance(center, new Vector2(unitAlly.position.x, unitAlly.position.z))
                                .Equals(1))
                            {
                                if (!mobNextTo.Contains(PlacementManager.instance.GetBoard()[j].monster))
                                {
                                    Debug.Log("oui");
                                    mobNextTo.Add(PlacementManager.instance.GetBoard()[j].monster);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (mobNextTo.Count>=numberCheck)
        {
            mobNextTo.Clear();
            return true;
        }
        
        return false;   
        
    }

    public void ActivateEffectWhenUnitDie()
    {
        foreach (var cases in PlacementManager.instance.GetBoard())
        {
            if (cases.monster.GetComponent<Monster>().HaveAnEffectThisPhase(enumEffectPhaseActivation.WhenAUnitDie))
            {
                cases.monster.GetComponent<Monster>().ActivateEffects(enumEffectPhaseActivation.WhenAUnitDie);
            }
        }
    }
    
    [PunRPC]
    private void RPC_PlayAnimation(int idEffect, float x, float y ,float z, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(CoroutinePlayAnimation(idEffect, x, y, z, duration));
    }

    IEnumerator CoroutinePlayAnimation(int idEffect, float x, float y ,float z, float duration)
    {
        instantiateEffect[idEffect].transform.position = new Vector3(x, y, z);
        instantiateEffect[idEffect].SetActive(true);
        
        PlayAllParticulesSystem(idEffect);
        
        yield return new WaitForSeconds(duration);
        
        instantiateEffect[idEffect].SetActive(false);
        instantiateEffect[idEffect].transform.position = new Vector3(x, y, z);
    }

    public void PlayAllParticulesSystem(int idEffect)
    {
        if (instantiateEffect[idEffect].GetComponent<ParticleSystem>() != null)
        {
            instantiateEffect[idEffect].GetComponent<ParticleSystem>().Play();
        }

        for (int k = 0; k <instantiateEffect[idEffect].transform.childCount; k++)
        {
            if (instantiateEffect[idEffect].transform.GetChild(k).GetComponent<ParticleSystem>() != null)
            {
                instantiateEffect[idEffect].transform.GetChild(k).GetComponent<ParticleSystem>().Play();
            }
        }

    }
}
