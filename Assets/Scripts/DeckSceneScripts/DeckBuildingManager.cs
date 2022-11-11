using System;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckBuildingManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] parchment;
    [SerializeField] private RectTransform pages;
    [SerializeField] private CanvasScaler canvas;
    [SerializeField] private List<GameObject> listDisplayedDeck;
    private Dictionary<int, int[]> listDeck = new Dictionary<int, int[]>();
    private int currentPages, choicePage2;
    private void Awake()
    {
        PhotonNetwork.Disconnect();
    }

    private void Start()
    {
        currentPages = 1;
        foreach (RectTransform parche in parchment)
        {
            parche.DOLocalMoveX(80, 1f).SetEase(Ease.OutBack).OnComplete(()=>parche.GetComponent<Button>().interactable = true);
        }
        
        listDeck[0] = LocalSaveManager.instance.user.diceDeck1;
        listDeck[1] = LocalSaveManager.instance.user.diceDeck2;
        listDeck[2] = LocalSaveManager.instance.user.diceDeck3;
        listDeck[3] = LocalSaveManager.instance.user.diceDeck4;
    }

    void DisplayDecks()
    {
        for (int i = 0; i < listDisplayedDeck.Count; i++)
        {
            if (!listDeck[i].Length.Equals(0))
            {
                listDisplayedDeck[i].SetActive(true);
                listDisplayedDeck[i].GetComponent<DeckDisplay>().deck = listDeck[i];
            }
        }
    }

    void ResetPage1()
    {
        foreach (RectTransform parche in parchment)
        {
            parche.DOLocalMoveX(975, 0f);
            parche.gameObject.SetActive(false);
        }
    }

    public void GoToPage2(int c)
    {
        pages.DOLocalMoveX(-canvas.referenceResolution.x, 0.5f).SetEase(Ease.Linear).OnComplete(()=>ResetPage1());
        choicePage2 = c;
        if (choicePage2==0)
        {
            DisplayDecks();
        }
        else
        {
            
        }
    }

    public void CreateDeck()
    {
        for (int i = 0; i < 4; i++)
        {
            if (listDeck[i].Length.Equals(0))
            {
                switch (i)
                {
                    case 0:
                        LocalSaveManager.instance.user.diceDeck1 = new int[8];
                        listDeck[0] = LocalSaveManager.instance.user.diceDeck1;
                        break;
                    case 1:
                        LocalSaveManager.instance.user.diceDeck2 = new int[8];
                        listDeck[1] = LocalSaveManager.instance.user.diceDeck2;
                        break;
                    case 2:
                        LocalSaveManager.instance.user.diceDeck3 = new int[8];
                        listDeck[2] = LocalSaveManager.instance.user.diceDeck3;
                        break;
                    case 3:
                        LocalSaveManager.instance.user.diceDeck4 = new int[8];
                        listDeck[3] = LocalSaveManager.instance.user.diceDeck4;
                        break;
                }
                
                listDisplayedDeck[i].SetActive(true);
                
                for (int j = 0; j < 8; j++)
                {
                    listDeck[i][j] = -1;
                }

                LocalSaveManager.instance.SaveUserData();
                return;
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
