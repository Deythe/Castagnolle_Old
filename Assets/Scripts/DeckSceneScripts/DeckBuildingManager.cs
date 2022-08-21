using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckBuildingManager : MonoBehaviour
{
    [SerializeField] private GameObject diceGroup;
    [SerializeField] private GameObject cardGroup;

    private void Awake()
    {
        PhotonNetwork.Disconnect();
    }

    public void ChangeCanvasUi()
    {
        diceGroup.SetActive(!diceGroup.activeSelf); 
        cardGroup.SetActive(!cardGroup.activeSelf);
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
