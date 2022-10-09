using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    
    public enum enumEffectConditionActivation
    {
        WhenThisUnitIsInvoke, WhenThisUnitKill, WhenAUnitDie, WhenItsDrawPhase, WhenThisUnitDie
    }
    
    public enum enumActionEffect
    {
        SelectAllyUnit, SelectAllyUnityButNotThisOne, SelectEnemyUnit, Heroism, HaveAMilkInGauge, SelectACard, KillCounter, DragUnit, Validate
    }

    public enum enumOrderPriority
    {
        LowPriority, MediumPriority, HighPriority
    }
    
    [SerializeField] private PhotonView view;
    [SerializeField] private List<GameObject> effectsList;
    [SerializeField] private List<GameObject> instantiateEffect;

    [SerializeField] private List<enumActionEffect> copyCurrentUnitAction;
    [SerializeField] private List<GameObject> heroismListMobCheck;
    
    [SerializeField] private GameObject currentUnit;
    [SerializeField] private GameObject unitTarget1;
    [SerializeField] private GameObject unitTarget2;
    [SerializeField] private Transform pooler;
    
    private MonstreData currentClick;
    
    private enumEffectConditionActivation currentEffectConditionActivation;

    private bool specialInvocation;
    private RaycastHit hit;
    private RoundManager.enumRoundState lastPhaseActivation; 
    private int i, j, numberCheckHeroism,numberIdAll = 0, numberCheck;

    public PhotonView View
    {
        get => view;
    }
    
    
    public bool p_specialInvocation
    {
        get => specialInvocation;
        set
        {
            specialInvocation = value;
        }
    }

    public GameObject p_currentUnit
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
    
    public void InstantiateAllEffect()
    {
        for (i = 0; i < effectsList.Count ; i++)
        {
            instantiateEffect.Add(Instantiate(effectsList[i],Vector3.zero, PlayerSetup.instance.transform.rotation, pooler));
            instantiateEffect[i].SetActive(false);
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
                            if (hit.collider.GetComponent<MonstreData>() != null)
                            {
                                currentClick = hit.collider.GetComponent<MonstreData>();
                                switch (RoundManager.instance.p_roundState)
                                {
                                    case RoundManager.enumRoundState.DrawPhase:
                                        if (currentClick.p_effect != null)
                                        {
                                            if (currentClick.photonView.AmOwner)
                                            {
                                                if (currentClick
                                                        .HaveAnEffectThisPhase(enumEffectConditionActivation
                                                            .WhenItsDrawPhase)
                                                    && currentClick.p_effect.GetIsActivable()
                                                    && !currentClick.p_effect.GetUsed())
                                                {
                                                    if (currentUnit != null)
                                                    {
                                                        currentUnit.GetComponent<MonstreData>().InitColorsTiles();
                                                    }
                                                    currentUnit = currentClick.gameObject;
                                                    currentUnit.GetComponent<MonstreData>().BeChoosen();
                                                    lastPhaseActivation = RoundManager.enumRoundState.DrawPhase;
                                                    UiManager.instance.EnableDisableScrollView(false);
                                                    UiManager.instance.EnableDisableBattleButton(false);
                                                    UiManager.instance.EnableDisableMenuNoChoice(true);
                                                    UiManager.instance.EnableDisableMenuYesChoice(true);
                                                }
                                            }
                                        }
                                        break;
                                        

                                    case RoundManager.enumRoundState.EffectPhase:
                                        if (copyCurrentUnitAction.Count > 0)
                                        {
                                            switch (copyCurrentUnitAction[0])
                                            {
                                                case enumActionEffect.SelectAllyUnityButNotThisOne:
                                                    if (hit.collider.GetComponent<PhotonView>().AmOwner)
                                                    {
                                                        if (!hit.collider.gameObject.Equals(currentUnit))
                                                        {
                                                            if (unitTarget1 == null)
                                                            {
                                                                unitTarget1 = hit.collider.gameObject;
                                                                unitTarget1.GetComponent<MonstreData>().BeChoosen();
                                                            }
                                                            else
                                                            {
                                                                unitTarget2 = hit.collider.gameObject;
                                                                unitTarget2.GetComponent<MonstreData>().BeChoosen();
                                                            }

                                                            copyCurrentUnitAction.RemoveAt(0);
                                                            CheckCondition();
                                                        }
                                                    }

                                                    break;

                                                case enumActionEffect.SelectAllyUnit:
                                                    if (hit.collider.GetComponent<PhotonView>().AmOwner)
                                                    {
                                                        if (unitTarget1 == null)
                                                        {
                                                            unitTarget1 = hit.collider.gameObject;
                                                            unitTarget1.GetComponent<MonstreData>().BeChoosen();
                                                        }
                                                        else
                                                        {
                                                            unitTarget2 = hit.collider.gameObject;
                                                            unitTarget2.GetComponent<MonstreData>().BeChoosen();
                                                        }

                                                        copyCurrentUnitAction.RemoveAt(0);
                                                        CheckCondition();

                                                    }

                                                    break;

                                                case enumActionEffect.SelectEnemyUnit:
                                                    if (!hit.collider.GetComponent<PhotonView>().AmOwner)
                                                    {
                                                        if (unitTarget1 == null)
                                                        {
                                                            unitTarget1 = hit.collider.gameObject;
                                                            unitTarget1.GetComponent<MonstreData>().BeChoosen();
                                                        }
                                                        else
                                                        {
                                                            unitTarget2 = hit.collider.gameObject;
                                                            unitTarget2.GetComponent<MonstreData>().BeChoosen();
                                                        }

                                                        copyCurrentUnitAction.RemoveAt(0);
                                                        CheckCondition();
                                                    }

                                                    break;
                                            }
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

    public void UnitSelected(enumEffectConditionActivation condition)
    {
        currentUnit.GetComponent<MonstreData>().BeChoosen();
        lastPhaseActivation = RoundManager.instance.p_roundState;
        currentEffectConditionActivation = condition;
        copyCurrentUnitAction = new List<enumActionEffect>(currentUnit
            .GetComponent<MonstreData>()
            .p_effect.GetActions());

        if (copyCurrentUnitAction.Count.Equals(0))
        {
            ActiveEffect();
        }
        else
        {
            CheckCondition();
        }

    }

    public void Action()
    {
        if (copyCurrentUnitAction.Count.Equals(0))
        {
            ActiveEffect();
            return;
        }

        if (copyCurrentUnitAction[0] == enumActionEffect.Validate)
        {
            copyCurrentUnitAction.RemoveAt(0);
            CheckCondition();
        }
    }

    void ActiveEffect()
    {
        Debug.Log( currentUnit.name +"Effet activated");
        UiManager.instance.EnableDisableMenuYesChoice(false); 
        UiManager.instance.EnableDisableMenuNoChoice(false); 
        currentUnit.GetComponent<MonstreData>().ActivateEffects(currentEffectConditionActivation);
    }

    public void CheckCondition()
    {
        if (copyCurrentUnitAction.Count == 0)
        {
            UiManager.instance.EnableDisableMenuYesChoice(true);
            return;
        }
      
        UiManager.instance.EnableDisableMenuYesChoice(false); 
        
        switch (copyCurrentUnitAction[0])
        {
            case enumActionEffect.SelectAllyUnityButNotThisOne:
            case enumActionEffect.SelectAllyUnit:
                RoundManager.instance.p_roundState = RoundManager.enumRoundState.EffectPhase;
                UiManager.instance.SetTextFeedBack(1);
                UiManager.instance.p_textFeedBack.enabled = true;
                break;
            case enumActionEffect.SelectEnemyUnit:
                RoundManager.instance.p_roundState = RoundManager.enumRoundState.EffectPhase;
                UiManager.instance.SetTextFeedBack(2);
                UiManager.instance.p_textFeedBack.enabled = true;
                break;
            case enumActionEffect.Heroism:
                for (i = copyCurrentUnitAction.Count-1; i >= 0; i--)
                {
                    if (copyCurrentUnitAction[i] == enumActionEffect.Heroism)
                    {
                        copyCurrentUnitAction.RemoveAt(i);
                    }
                }
                
                if (copyCurrentUnitAction.Count == 0)
                {
                    ActiveEffect();
                    return;
                }
                
                CheckCondition();
                break;
            case enumActionEffect.KillCounter:
                currentUnit.GetComponent<MonstreData>().p_effect.GetActions().RemoveAt(0);
                copyCurrentUnitAction.RemoveAt(0);
                if (copyCurrentUnitAction.Count.Equals(0))
                {
                    ActiveEffect();
                }
                else
                {
                    if (copyCurrentUnitAction[0] != enumActionEffect.KillCounter)
                    {
                        ActiveEffect();
                    }
                    else
                    {
                        CancelSelection();
                    }   
                }

                break;
            case enumActionEffect.DragUnit:
                RoundManager.instance.p_roundState = RoundManager.enumRoundState.EffectPhase;
                UiManager.instance.p_textFeedBack.enabled = true;
                UiManager.instance.SetTextFeedBack(0);
                specialInvocation = true;
                ActiveEffect();
                break;
            case enumActionEffect.HaveAMilkInGauge:
                RoundManager.instance.p_roundState = RoundManager.enumRoundState.EffectPhase;
                copyCurrentUnitAction.RemoveAt(0);
                if (copyCurrentUnitAction.Count.Equals(0))
                {
                    ActiveEffect();
                }
                else
                {
                    CheckCondition();
                }
                break;
            case enumActionEffect.SelectACard:
                RoundManager.instance.p_roundState = RoundManager.enumRoundState.EffectPhase;
                ActiveEffect();
                break;
            case enumActionEffect.Validate:
                UiManager.instance.EnableDisableMenuYesChoice(true);
                break;
        }
    }

    public void CancelSelection()
    {
        ClearUnits();
        specialInvocation = false;

        for (i = 0; i < UiManager.instance.p_carListToBeSelected.childCount; i++)
        {
            UiManager.instance.p_carListToBeSelected.GetChild(i).gameObject.SetActive(false);
        }
        
        if (lastPhaseActivation == RoundManager.enumRoundState.DragUnitPhase)
        {
            lastPhaseActivation = RoundManager.enumRoundState.DrawPhase;
        }else if (lastPhaseActivation == RoundManager.enumRoundState.ThrowPhase)
        {
            lastPhaseActivation = RoundManager.enumRoundState.WaitPhase;
        }
        
        RoundManager.instance.p_roundState = lastPhaseActivation;
    }

    public void ClearUnits()
    {
        UiManager.instance.p_textFeedBack.enabled = false;
        if (unitTarget1 != null)
        {
            unitTarget1.GetComponent<MonstreData>().InitColorsTiles();
        }
        
        if (unitTarget2 != null)
        {
            unitTarget2.GetComponent<MonstreData>().InitColorsTiles();
        }

        if (currentUnit!=null)
        {
            currentUnit.GetComponent<MonstreData>().InitColorsTiles();
        }

        unitTarget1 = null;
        unitTarget2 = null;
        currentUnit = null;
    }

    public void CheckAllHeroism()
    {
        for (int j = 0; j < PlacementManager.instance.p_board.Count; j++)
        {
            if (PlacementManager.instance.p_board[j].monster.GetComponent<MonstreData>().photonView.AmOwner 
                && PlacementManager.instance.p_board[j].monster.GetComponent<MonstreData>().p_effect!=null 
                && PlacementManager.instance.p_board[j].monster.GetComponent<MonstreData>().p_effect.GetActions()
                    .Contains(enumActionEffect.Heroism))
            {
                CheckHeroism(PlacementManager.instance.p_board[j].monster.transform);
            }   
        }
    }
    
    public void CheckAllHaveAMilkInGauge()
    {
        for (int j = 0; j < PlacementManager.instance.p_board.Count; j++)
        {
            if (PlacementManager.instance.p_board[j].monster.GetComponent<MonstreData>().photonView.AmOwner 
                && PlacementManager.instance.p_board[j].monster.GetComponent<MonstreData>().p_effect!=null 
                && PlacementManager.instance.p_board[j].monster.GetComponent<MonstreData>().p_effect.GetActions()
                    .Contains(enumActionEffect.HaveAMilkInGauge))
            {
                CheckMilkInGauge(PlacementManager.instance.p_board[j].monster.transform);
            }   
        }
    }
    
    void CheckHeroism(Transform go)
    {
        numberCheck = 0;
        heroismListMobCheck.Clear();
        
        foreach (var check in go.GetComponent<MonstreData>().p_effect.GetActions())
        {
            if (check == enumActionEffect.Heroism)
            {
                numberCheck++;
            }
        }
        
        for (i = 0; i < numberCheck; i++)
        {
            for (j = 0; j < PlacementManager.instance.p_board.Count; j++)
            {
                foreach (var unitAlly in go.GetComponent<MonstreData>().GetCenters())
                {
                    if (!PlacementManager.instance.p_board[j].monster.GetComponent<PhotonView>().AmOwner)
                    {
                        foreach (var center in PlacementManager.instance.p_board[j].emplacement)
                        {
                            if (Vector2.Distance(center, new Vector2(unitAlly.position.x, unitAlly.position.z))
                                .Equals(1))
                            {
                                if (!heroismListMobCheck.Contains(PlacementManager.instance.p_board[j].monster))
                                {
                                    heroismListMobCheck.Add(PlacementManager.instance.p_board[j].monster);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (heroismListMobCheck.Count>=numberCheck)
        {
            heroismListMobCheck.Clear();
            go.GetComponent<MonstreData>().p_effect.SetIsActivable( true);
            go.GetComponent<MonstreData>().p_model.layer = 7;
            return;
        }
        
        go.GetComponent<MonstreData>().p_effect.SetIsActivable(false);
        go.GetComponent<MonstreData>().p_model.layer = 6;
    }

    void CheckMilkInGauge(Transform go)
    {
        if (go.GetComponent<MonstreData>().p_effect.GetActions().Contains(enumActionEffect.HaveAMilkInGauge))
        {
            foreach (var milks in DiceManager.instance.p_diceGauge)
            {
                if (milks == DiceListScriptable.enumRessources.Milk)
                {
                    go.GetComponent<MonstreData>().p_effect.SetIsActivable( true);
                    go.GetComponent<MonstreData>().p_model.layer = 7;
                    return;
                }
            }
            
            go.GetComponent<MonstreData>().p_effect.SetIsActivable(false);
            go.GetComponent<MonstreData>().p_model.layer = 6;
        }
    }

    public void ActivateEffectWhenUnitDie()
    {
        foreach (var cases in PlacementManager.instance.p_board)
        {
            if (cases.monster.GetComponent<MonstreData>().HaveAnEffectThisPhase(enumEffectConditionActivation.WhenAUnitDie) 
                && cases.monster.GetComponent<MonstreData>().p_effect.GetIsActivable()
                && !cases.monster.GetComponent<MonstreData>().p_effect.GetUsed())
            {
                if (cases.monster.GetPhotonView().AmOwner)
                {
                    currentUnit = cases.monster;
                    UnitSelected(enumEffectConditionActivation.WhenAUnitDie);
                }
                else
                {
                    switch (CastagneManager.instance.p_result)
                    {
                        case 0:
                            view.RPC("RPC_ActivateEffectWhenUnitDieEnemy", RpcTarget.Others, cases.monster.GetComponent<MonstreData>().p_id, 0, CastagneManager.instance.p_result);
                            break;
                        case >0:
                           view.RPC("RPC_ActivateEffectWhenUnitDieEnemy", RpcTarget.Others, cases.monster.GetComponent<MonstreData>().p_id, CastagneManager.instance.p_unitSelected.GetComponent<MonstreData>().p_id, CastagneManager.instance.p_result);
                           break;
                       
                        case <0:
                            view.RPC("RPC_ActivateEffectWhenUnitDieEnemy", RpcTarget.Others, cases.monster.GetComponent<MonstreData>().p_id, CastagneManager.instance.p_unitTarget.GetComponent<MonstreData>().p_id, CastagneManager.instance.p_result);
                            break;
                    }
                }
            }
        }
    }
    
    [PunRPC]
    private void RPC_ActivateEffectWhenUnitDieEnemy(int idUnitWhichUseEffect, int idUnitSurvived, int result)
    {
        Debug.Log("Effet Unit distant");
        currentUnit = PlacementManager.instance.FindMobWithID(idUnitWhichUseEffect).gameObject;
        CastagneManager.instance.p_result = result;
        switch (result)
        {
            case >0:
                CastagneManager.instance.p_unitTarget = PlacementManager.instance.FindMobWithID(idUnitSurvived).gameObject;
                break;
                       
            case <0:
                CastagneManager.instance.p_unitSelected = PlacementManager.instance.FindMobWithID(idUnitSurvived).gameObject;
                break;   
        }
        CastagneManager.instance.p_result = result;
        UnitSelected(enumEffectConditionActivation.WhenAUnitDie);
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
