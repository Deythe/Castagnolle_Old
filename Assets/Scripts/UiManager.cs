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
    [SerializeField] private TMP_Text numberRound;
    
    [SerializeField] private TMP_Text dice1;
    [SerializeField] private TMP_Text dice2;
    [SerializeField] private TMP_Text dice3;
    [SerializeField] private TMP_Text FPS;
    
    [SerializeField] private GameObject uiPlayerTurn;
    [SerializeField] private GameObject endTurn;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject menuBattlePhase;
    
    [SerializeField] private GameObject menuYesChoice;
    [SerializeField] private GameObject menuNoChoice;
    
    [SerializeField] private GameObject rotationMenu;

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
        EnableDisableRotateMenu();

        FPS.text = "" + 1 / Time.deltaTime;
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
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==4)
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
    
    void EnableDisableRotateMenu()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==3)
        {
            rotationMenu.SetActive(true);
        }
        else
        {
            rotationMenu.SetActive(false);
        }
    }
    
    void EnableDisableEndTurn()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==5)
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
        dice1.text = ""+DiceManager.instance.GetDiceDeck()[0];
        dice2.text = ""+DiceManager.instance.GetDiceDeck()[1];
        dice3.text = ""+DiceManager.instance.GetDiceDeck()[2];
    }
    
    
    
}
