using System;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DeckBuildingManager : MonoBehaviour
{
    public static DeckBuildingManager instance;
    
    [SerializeField] private RectTransform[] parchment;
    [SerializeField] private RectTransform pages, page2And3, cardListDisplayContent;
    [SerializeField] private CanvasScaler canvas;
    [SerializeField] private List<GameObject> listDisplayedCardsDeck, listDisplayedDicesDeck, listDecksMenu, listElementForP2, listElementForP3, listSwitch;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject _bigCard;
    private GameObject cardListPivot;
    private Dictionary<int, int[]> listCardsDeck = new Dictionary<int, int[]>(), listDicesDeck = new Dictionary<int, int[]>();
    private int currentPage, choicePage2 =0;
    private int[] currentDeckModifing;

    public GameObject bigCard
    {
        get => _bigCard;
        set
        {
            _bigCard = value;
        }
    }
    private void Awake()
    {
        PhotonNetwork.Disconnect();
        instance = this;
    }

    private void Start()
    {
        currentPage = 1;
        ShowParchment();
        
        listCardsDeck[0] = LocalSaveManager.instance.user.monsterDeck1;
        listCardsDeck[1] = LocalSaveManager.instance.user.monsterDeck2;
        listCardsDeck[2] = LocalSaveManager.instance.user.monsterDeck3;
        listCardsDeck[3] = LocalSaveManager.instance.user.monsterDeck4;
        
        listDicesDeck[0] = LocalSaveManager.instance.user.diceDeck1;
        listDicesDeck[1] = LocalSaveManager.instance.user.diceDeck2;
        listDicesDeck[2] = LocalSaveManager.instance.user.diceDeck3;
        listDicesDeck[3] = LocalSaveManager.instance.user.diceDeck4;
        
        DisplayCardList();
    }

    void DisplayCardList()
    {
        for (int i = 0; i < LocalSaveManager.instance.unitListScriptable.cards.Count; i++)
        {
            cardListPivot = new GameObject();
            cardListPivot.transform.SetParent(cardListDisplayContent);
            cardListPivot.AddComponent<Image>();
            cardListPivot.GetComponent<Image>().sprite =
                LocalSaveManager.instance.unitListScriptable.cards[i].cardStats;
            cardListPivot.AddComponent<CardsMovable>();
            cardListPivot.GetComponent<CardsMovable>().index = i;
            cardListPivot.GetComponent<CardsMovable>().miniature = LocalSaveManager.instance.unitListScriptable.cards[i]
                .miniaCard.GetComponent<Image>().sprite;
        }
    }
    
    void DisplayCurrentDeck()
    {
        for (int i = 0; i < LocalSaveManager.instance.unitListScriptable.cards.Count; i++)
        {
            cardListPivot = Instantiate(new GameObject(), cardListDisplayContent);
            cardListPivot.AddComponent<Image>();
            cardListPivot.GetComponent<Image>().sprite =
                LocalSaveManager.instance.unitListScriptable.cards[i].cardStats;
        }
    }

    public void SwitchDiceAndCardsMenu()
    {
        listDecksMenu[choicePage2].SetActive(false);
        choicePage2 = (int)slider.value;
        
        listDecksMenu[choicePage2].SetActive(true);
        if (slider.value.Equals(0))
        {
            listSwitch[0].SetActive(true);
            listSwitch[1].SetActive(false);
            return;   
        }
        
        listSwitch[1].SetActive(true);
        listSwitch[0].SetActive(false);
        
    }

    void ShowParchment()
    {
        foreach (RectTransform parche in parchment)
        {
            parche.gameObject.SetActive(true);
            parche.DOLocalMoveX(80, 1f).SetEase(Ease.OutBack).OnComplete(()=>parche.GetComponent<Button>().interactable = true);
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

    void DisplayCardsDecks()
    {
        for (int i = 0; i < listDisplayedDicesDeck.Count; i++)
        {
            if (!listCardsDeck[i].Length.Equals(0))
            {
                listDisplayedCardsDeck[i].SetActive(true);
                listDisplayedCardsDeck[i].GetComponent<DeckDisplay>().deck = listCardsDeck[i];
            }
        }
    }

    void DisplayDiceDecks()
    {
        
        for (int i = 0; i < listDisplayedDicesDeck.Count; i++)
        {
            if (!listDicesDeck[i].Length.Equals(0))
            {
                listDisplayedDicesDeck[i].SetActive(true);
                listDisplayedDicesDeck[i].GetComponent<DeckDisplay>().deck = listDicesDeck[i];
            }
        }
    }
    
    public void CreateDiceDeck()
    {
        for (int i = 0; i < 4; i++)
        {
            if (listCardsDeck[i].Length.Equals(0))
            {
                switch (i)
                {
                    case 0:
                        LocalSaveManager.instance.user.monsterDeck1 = new int[8];
                        listCardsDeck[0] = LocalSaveManager.instance.user.monsterDeck1;
                        break;
                    case 1:
                        LocalSaveManager.instance.user.monsterDeck2 = new int[8];
                        listCardsDeck[1] = LocalSaveManager.instance.user.monsterDeck2;
                        break;
                    case 2:
                        LocalSaveManager.instance.user.monsterDeck3 = new int[8];
                        listCardsDeck[2] = LocalSaveManager.instance.user.monsterDeck3;
                        break;
                    case 3:
                        LocalSaveManager.instance.user.monsterDeck4 = new int[8];
                        listCardsDeck[3] = LocalSaveManager.instance.user.monsterDeck4;
                        break;
                }
                
                listDisplayedCardsDeck[i].SetActive(true);
                
                for (int j = 0; j < 8; j++)
                {
                    listCardsDeck[i][j] = -1;
                }

                LocalSaveManager.instance.SaveUserData();
                GoToPage3(i);
                return;
            }
        }
    }
    
    public void Return()
    {
        if (currentPage == 2)
        {
            pages.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).OnComplete(()=>ShowParchment());

        }else if (currentPage == 3)
        {
            currentPage = 2;
            page2And3.DOLocalMoveX(canvas.referenceResolution.x, 0.5f).SetEase(Ease.Linear);
            DisplayElementForP2AndP3(true);
        }
    }

    void DisplayElementForP2AndP3(bool b)
    {
        for (int i = 0; i < listElementForP2.Count; i++)
        {
            listElementForP2[i].SetActive(b);
        }
        
        for (int i = 0; i < listElementForP3.Count; i++)
        {
            listElementForP3[i].SetActive(!b);
        }
    }
    
    public void GoToPage2(int c)
    {
        slider.value = c;
        SwitchDiceAndCardsMenu();
        currentPage = 2;

        DisplayElementForP2AndP3(true);
        DisplayCardsDecks();
        DisplayDiceDecks();
        pages.DOLocalMoveX(-canvas.referenceResolution.x, 0.5f).SetEase(Ease.Linear).OnComplete(()=>ResetPage1());
    }
    
    public void GoToPage3(int currentDeckIndex)
    {
        currentPage = 3;
        if (choicePage2 == 0)
        {
            currentDeckModifing = listCardsDeck[currentDeckIndex];
        }
        else
        {
            currentDeckModifing = listDicesDeck[currentDeckIndex];
        }

        page2And3.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear);
        
        DisplayElementForP2AndP3(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
