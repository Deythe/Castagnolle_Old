using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckBuildingManager : MonoBehaviour
{
    public static DeckBuildingManager instance;
    
    [SerializeField] private RectTransform[] parchment;
    [SerializeField] private RectTransform pages, page2And3, cardListDisplayContent, _diceListDisplayContent, miniatureMovable, _diceDetails;
    [SerializeField] private CanvasScaler canvas;

    [SerializeField] private List<GameObject> listDisplayedCardsDeck,
        listDisplayedDicesDeck,
        listDecksMenu,
        listElementForP2,
        listElementForP3,
        listSwitch,
        listP3Pages;
    
    [SerializeField] private List<GameObject> listDisplayedCurrentCardDeckModifying, listDisplayedCurrentDicesDeckModifying;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject _bigCard;
    [SerializeField] private TMP_Text totalTextObjectInCurrentDeckModifying;
    [SerializeField] private TMP_InputField nameOfCurrentDeckModifying;
    [SerializeField] private ScrollRect _scroll;
    
    private GameObject cardListPivot;
    private Dictionary<int, int[]> listCardsDeck = new Dictionary<int, int[]>(), listDicesDeck = new Dictionary<int, int[]>();

    private int currentPage,
        choicePage2 = 0,
        _currentIndexObjectMovable = 0,
        indexCurrentDeckModifying,
        _totalObjectInCurrentDeckModifying;
    private int[] currentDeckModifying;
    private bool _isMovingACard, _hoverDropZone;
    private Color transparency = new Color(1,1,1,0.25f);

    public RectTransform diceListDisplayContent
    {
        get => _diceListDisplayContent;
    }
    int totalObjectInCurrentDeckModifying
    {
        get => _totalObjectInCurrentDeckModifying;
        set
        {
            _totalObjectInCurrentDeckModifying = value;
            totalTextObjectInCurrentDeckModifying.text = _totalObjectInCurrentDeckModifying + "/" + currentDeckModifying.Length;
        }
    }

    public int currentIndexObjectMovable
    {
        get => _currentIndexObjectMovable;
        set
        {
            _currentIndexObjectMovable = value;
        }
    }
    public bool hoverDropZone
    {
        get => _hoverDropZone;
        set
        {
            _hoverDropZone = value;
        }
    }
    
    public Sprite cardMovableSprite
    {
        get => miniatureMovable.GetComponent<Image>().sprite;
        set
        {
            miniatureMovable.GetComponent<Image>().sprite = value;
        }
    } 
    
    public bool isMovingACard
    {
        get => _isMovingACard;
        set
        {
            _isMovingACard = value;
            miniatureMovable.gameObject.SetActive(_isMovingACard);
            _scroll.vertical = !isMovingACard;
            
        }
    }
    public RectTransform diceDetails
    {
        get => _diceDetails;
        set
        {
            _diceDetails = value;
        }
    }
    
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
        DisplayDiceList();
    }

    private void Update()
    {
        if (currentPage.Equals(3) && _isMovingACard && Input.touchCount > 0)
        {
            miniatureMovable.position = Input.GetTouch(0).position;
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (hoverDropZone)
                {
                    if(choicePage2==0)
                    {
                        PutInCardDeck(_currentIndexObjectMovable);
                    }
                    else
                    {
                        PutInDiceDeck(_currentIndexObjectMovable);
                    }

                    _currentIndexObjectMovable = 0;
                    totalObjectInCurrentDeckModifying++;
                }
                
                hoverDropZone = false;
                isMovingACard = false;
            }
        }
    }

    void EnableDisableCardInListOfCardInGame(bool b, int index)
    {
        for (int i = 0; i < cardListDisplayContent.childCount; i++)
        {
            if (i.Equals(index))
            {
                cardListDisplayContent.GetChild(i).GetComponent<CardsInBuildingDeck>().enabled = b;
                if (b)
                {
                    cardListDisplayContent.GetChild(i).GetComponent<Image>().color = Color.white;
                }
                else
                {
                    cardListDisplayContent.GetChild(i).GetComponent<Image>().color = transparency;
                }
               
                return;
            }
        }
    }
    void EnableDisableAllCardInList(bool b)
    {
        for (int i = 0; i < cardListDisplayContent.childCount; i++)
        {
            if (b)
            {
                cardListDisplayContent.GetChild(i).GetComponent<Image>().color = Color.white;
                cardListDisplayContent.GetChild(i).GetComponent<CardsInBuildingDeck>().enabled = true;
            }
            else
            {
                cardListDisplayContent.GetChild(i).GetComponent<Image>().color = transparency;
                cardListDisplayContent.GetChild(i).GetComponent<CardsInBuildingDeck>().enabled = false;
            }
        }
    }
    void PutInCardDeck(int i)
    {
        for (int j = 0; j < currentDeckModifying.Length; j++)
        {
            if (currentDeckModifying[j].Equals(-1))
            {
                currentDeckModifying[j] = i;
                listDisplayedCurrentCardDeckModifying[j].GetComponent<Image>().sprite =
                    miniatureMovable.GetComponent<Image>().sprite;
                listDisplayedCurrentCardDeckModifying[j].SetActive(true);
                LocalSaveManager.instance.SaveUserData();
                EnableDisableCardInListOfCardInGame(false, i);
                return;
            }
        }
    }
    
    void PutInDiceDeck(int i)
    {
        for (int j = 0; j < currentDeckModifying.Length; j++)
        {
            if (currentDeckModifying[j].Equals(-1))
            {
                currentDeckModifying[j] = i;
                listDisplayedCurrentDicesDeckModifying[j].GetComponent<Image>().sprite =
                    miniatureMovable.GetComponent<Image>().sprite;
                listDisplayedCurrentDicesDeckModifying[j].SetActive(true);
                LocalSaveManager.instance.SaveUserData();
                //EnableDisableCardInListOfCardInGame(false, i);
                return;
            }
        }
    }
    void DisplayCardList()
    {
        for (int i = 0; i < LocalSaveManager.instance.unitList.cards.Count; i++)
        {
            cardListPivot = new GameObject();
            cardListPivot.transform.SetParent(cardListDisplayContent);
            cardListPivot.AddComponent<Image>();
            cardListPivot.GetComponent<Image>().sprite =
                LocalSaveManager.instance.unitList.cards[i].cardStats;
            cardListPivot.AddComponent<CardsInBuildingDeck>();
            cardListPivot.GetComponent<CardsInBuildingDeck>().index = i;
            cardListPivot.GetComponent<CardsInBuildingDeck>().miniature = LocalSaveManager.instance.unitList.cards[i]
                .miniaCard.GetComponent<Image>().sprite;
        }
    }
    
    void DisplayDiceList()
    {
        for (int i = 0; i < LocalSaveManager.instance.dicesList.dicesList.Count; i++)
        {
            cardListPivot = new GameObject();
            cardListPivot.transform.SetParent(_diceListDisplayContent);
            cardListPivot.AddComponent<Image>();
            cardListPivot.GetComponent<Image>().sprite =
                LocalSaveManager.instance.dicesList.dicesList[i].sprite;
            cardListPivot.AddComponent<DicesInBuildingDeck>();
            cardListPivot.GetComponent<DicesInBuildingDeck>().index = i;
            cardListPivot.GetComponent<DicesInBuildingDeck>().sprite = LocalSaveManager.instance.dicesList.dicesList[i].sprite;
        }
    }

    public void SavedCurrentDeckName()
    {
        LocalSaveManager.instance.user.cardDeckName[indexCurrentDeckModifying] = nameOfCurrentDeckModifying.text;
        LocalSaveManager.instance.SaveUserData();
    }
    public void RemoveFromCardDeck(int index)
    {
        EnableDisableCardInListOfCardInGame(true, currentDeckModifying[index]);
        currentDeckModifying[index] = -1;
        listDisplayedCurrentCardDeckModifying[index].SetActive(false);
        totalObjectInCurrentDeckModifying--;
        LocalSaveManager.instance.SaveUserData();
    }
    
    public void RemoveFromDicesDeck(int index)
    {
        currentDeckModifying[index] = -1;
        listDisplayedCurrentDicesDeckModifying[index].SetActive(false);
        totalObjectInCurrentDeckModifying--;
        LocalSaveManager.instance.SaveUserData();
    }

    void ResetDisplayCurrentDeck()
    {
        for (int i = 0; i < listDisplayedCurrentCardDeckModifying.Count; i++)
        {
            if (!currentDeckModifying[i].Equals(-1))
            {
                listDisplayedCurrentCardDeckModifying[i].GetComponent<Image>().sprite = null;
                listDisplayedCurrentDicesDeckModifying[i].GetComponent<Image>().sprite = null;
                
                listDisplayedCurrentDicesDeckModifying[i].SetActive(false);
                listDisplayedCurrentCardDeckModifying[i].SetActive(false);
            }
        }
    }

    void DisplayCurrentCardsDeck()
    {
        totalObjectInCurrentDeckModifying = 0;
        for (int i = 0; i < currentDeckModifying.Length; i++)
        {
            if (!currentDeckModifying[i].Equals(-1))
            {
                listDisplayedCurrentCardDeckModifying[i].GetComponent<Image>().sprite = LocalSaveManager.instance
                    .unitList.cards[currentDeckModifying[i]].miniaCard.GetComponent<Image>().sprite;
                listDisplayedCurrentCardDeckModifying[i].SetActive(true);
                totalObjectInCurrentDeckModifying++;
            }
        }
    }
    
    void DisplayCurrentDicesDeck()
    {
        totalObjectInCurrentDeckModifying = 0;
        for (int i = 0; i < currentDeckModifying.Length; i++)
        {
            if (!currentDeckModifying[i].Equals(-1))
            {
                listDisplayedCurrentDicesDeckModifying[i].GetComponent<Image>().sprite = LocalSaveManager.instance
                    .dicesList.dicesList[currentDeckModifying[i]].sprite;
                listDisplayedCurrentDicesDeckModifying[i].SetActive(true);
                totalObjectInCurrentDeckModifying++;
            }
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
            listP3Pages[0].SetActive(true);
            listP3Pages[1].SetActive(false);
            return;   
        }
        
        listSwitch[1].SetActive(true);
        listSwitch[0].SetActive(false);
        listP3Pages[1].SetActive(true);
        listP3Pages[0].SetActive(false);
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
        for (int i = 0; i < listDisplayedCardsDeck.Count; i++)
        {
            if (!listCardsDeck[i].Length.Equals(0))
            {
                listDisplayedCardsDeck[i].SetActive(true);
                listDisplayedCardsDeck[i].GetComponent<DeckDisplay>().deck = listCardsDeck[i];
                listDisplayedCardsDeck[i].GetComponent<DeckDisplay>().UpdateCounterObject();
                listDisplayedCardsDeck[i].GetComponent<DeckDisplay>().UpdateDeckName(LocalSaveManager.instance.user.cardDeckName[i]);
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
                listDisplayedDicesDeck[i].GetComponent<DeckDisplay>().UpdateCounterObject();
                listDisplayedDicesDeck[i].GetComponent<DeckDisplay>().UpdateDeckName(LocalSaveManager.instance.user.cardDeckName[i]);
            }
        }
    }
    
    public void CreateCardDeck()
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
    
    public void CreateDiceDeck()
    {
        for (int i = 0; i < 4; i++)
        {
            if (listDicesDeck[i].Length.Equals(0))
            {
                switch (i)
                {
                    case 0:
                        LocalSaveManager.instance.user.diceDeck1 = new int[9];
                        listDicesDeck[0] = LocalSaveManager.instance.user.diceDeck1;
                        break;
                    case 1:
                        LocalSaveManager.instance.user.diceDeck2 = new int[9];
                        listDicesDeck[1] = LocalSaveManager.instance.user.diceDeck2;
                        break;
                    case 2:
                        LocalSaveManager.instance.user.diceDeck3 = new int[9];
                        listDicesDeck[2] = LocalSaveManager.instance.user.diceDeck3;
                        break;
                    case 3:
                        LocalSaveManager.instance.user.diceDeck4 = new int[9];
                        listDicesDeck[3] = LocalSaveManager.instance.user.diceDeck4;
                        break;
                }
                
                listDisplayedDicesDeck[i].SetActive(true);
                
                for (int j = 0; j < 9; j++)
                {
                    listDicesDeck[i][j] = -1;
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
            DisplayCardsDecks();
            DisplayDiceDecks();
            ResetDisplayCurrentDeck();
            EnableDisableAllCardInList(true);
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
        indexCurrentDeckModifying = currentDeckIndex;
        if (choicePage2 == 0)
        {
            currentDeckModifying = listCardsDeck[currentDeckIndex];
            DisplayCurrentCardsDeck();
            for(int i = 0; i < currentDeckModifying.Length; i++)
            {
                if (!currentDeckModifying[i].Equals(-1))
                {
                    EnableDisableCardInListOfCardInGame(false, currentDeckModifying[i]);
                }
            }

            nameOfCurrentDeckModifying.text = LocalSaveManager.instance.user.cardDeckName[currentDeckIndex];
        }
        else
        {
            currentDeckModifying = listDicesDeck[currentDeckIndex];
            DisplayCurrentDicesDeck();
            nameOfCurrentDeckModifying.text = LocalSaveManager.instance.user.diceDeckName[currentDeckIndex];
        }

        page2And3.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear);
        
        DisplayElementForP2AndP3(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
