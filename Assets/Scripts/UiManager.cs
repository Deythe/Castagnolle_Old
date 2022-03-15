using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    [SerializeField] private TMP_Text numberRound;
    
    [SerializeField] private TMP_Text dice1;
    [SerializeField] private TMP_Text dice2;
    [SerializeField] private TMP_Text dice3;
    [SerializeField] private TMP_Text fps;
    [SerializeField] private TMP_Text hp;
    
    [SerializeField] private GameObject uiPlayerTurn;
    [SerializeField] private GameObject endTurn;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject menuBattlePhase;
    
    [SerializeField] private GameObject menuYesChoice;
    [SerializeField] private GameObject menuNoChoice;
    
    [SerializeField] private RectTransform content;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        ChangeRoundUI();
        
        EnableDisableThrowDiceButton();
        EnableDisableDice();
        EnableDisableScrollView();
        EnableDisableEndTurn();
        EnableDisableBattleButton();
        EnableDisableMenuYesChoice();
        EnableDisableMenuNoChoice();
        
        UpdateDice();
        UpdateFPS();
        UpdateHp();
    }

    void UpdateHp()
    {
        hp.text = "PV : " + LifeManager.instance.GetOwnLife();
    }
    void UpdateFPS()
    {
        fps.text = "" + 1 / Time.deltaTime;
    }

    public void UpdateListCard()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        foreach (var unit in DeckManager.instance.GetMonsters())
        {
            Instantiate(unit,content);
        }
    }

    void EnableDisableThrowDiceButton()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==0)
        {
            uiPlayerTurn.SetActive(true);
        }
        else
        {
            uiPlayerTurn.SetActive(false);
        }
    }
    
    void EnableDisableScrollView()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==1 && !DiceManager.instance.DeckEmpy())
        {
            scrollView.SetActive(true);
        }
        else
        {
            scrollView.SetActive(false);
        }
    }
    
    void EnableDisableMenuYesChoice()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==4 && BattlePhaseManager.instance.GetIsAttacking())
        {
            menuYesChoice.SetActive(true);
        }
        else
        {
            menuYesChoice.SetActive(false);
        }
    }
    
    void EnableDisableMenuNoChoice()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==4 || RoundManager.instance.GetStateRound()==2)
        {
            menuNoChoice.SetActive(true);
        }
        else
        {
            menuNoChoice.SetActive(false);
        }
    }
    void EnableDisableBattleButton()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==1)
        {
            menuBattlePhase.SetActive(true);
        }
        else
        {
            menuBattlePhase.SetActive(false);
        }
    }
    void EnableDisableEndTurn()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==3)
        {
            endTurn.SetActive(true);
        }
        else
        {
            endTurn.SetActive(false);
        }
    }
    

    void EnableDisableDice()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && !DiceManager.instance.DeckEmpy())
        {
            dice1.enabled = true;
            dice2.enabled = true;
            dice3.enabled = true;
        }
        else
        {
            dice1.enabled = false;
            dice2.enabled = false;
            dice3.enabled = false;
        }
    }
    
    void ChangeRoundUI()
    {
        numberRound.text = "" + (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"];
    }

    void UpdateDice()
    {
        dice1.text = ""+DiceManager.instance.GetDiceChoosen()[0];
        dice2.text = ""+DiceManager.instance.GetDiceChoosen()[1];
        dice3.text = ""+DiceManager.instance.GetDiceChoosen()[2];
    }
    
    
    
}
