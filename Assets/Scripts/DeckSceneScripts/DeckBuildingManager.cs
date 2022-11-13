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
    [SerializeField] private RectTransform pages, page2And3, cardListDisplayContent, miniatureMovable;
    [SerializeField] private CanvasScaler canvas;
    [SerializeField] private List<GameObject> listDisplayedCardsDeck, listDisplayedDicesDeck, listDecksMenu, listElementForP2, listElementForP3, listSwitch;
    [SerializeField] private List<GameObject> listDisplayedCurrentDeckModifying;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject _bigCard;
    [SerializeField] private TMP_Text totalTextMonsterInCurrentDeckModifying;
    [SerializeField] private TMP_InputField nameOfCurrentDeckModifying;
    [SerializeField] private ScrollRect _scroll;
    
    private int _totalMonsterInCurrentDeckModifying;
    private GameObject cardListPivot;
    private Dictionary<int, int[]> listCardsDeck = new Dictionary<int, int[]>(), listDicesDeck = new Dictionary<int, int[]>();
    private int currentPage, choicePage2 =0, _currentIndexCardMovable = 0, indexCurrentDeckModifying;
    private int[] currentDeckModifying;
    private bool _isMovingACard, _hoverDropZone;
    private Color transparency = new Color(1,1,1,0.25f);
    int totalMonsterInCurrentDeckModifying
    {
        get => _totalMonsterInCurrentDeckModifying;
        set
        {
            _totalMonsterInCurrentDeckModifying = value;
            totalTextMonsterInCurrentDeckModifying.text = _totalMonsterInCurrentDeckModifying + "/8";
        }
    }
    public int currentIndexCardMovable
    {
        get => _currentIndexCardMovable;
        set
        {
            _currentIndexCardMovable = value;
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

    private void Update()
    {
        if (currentPage.Equals(3) && _isMovingACard && Input.touchCount > 0)
        {
            miniatureMovable.position = Input.GetTouch(0).position;
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (hoverDropZone)
                {
                    PutInCardDeck(_currentIndexCardMovable);
                    _currentIndexCardMovable = 0;
                    totalMonsterInCurrentDeckModifying++;
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
                cardListDisplayContent.GetChild(i).GetComponent<CardsMovable>().enabled = b;
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
                cardListDisplayContent.GetChild(i).GetComponent<CardsMovable>().enabled = true;
            }
            else
            {
                cardListDisplayContent.GetChild(i).GetComponent<Image>().color = transparency;
                cardListDisplayContent.GetChild(i).GetComponent<CardsMovable>().enabled = false;
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
                listDisplayedCurrentDeckModifying[j].GetComponent<Image>().sprite =
                    miniatureMovable.GetComponent<Image>().sprite;
                listDisplayedCurrentDeckModifying[j].SetActive(true);
                LocalSaveManager.instance.SaveUserData();
                EnableDisableCardInListOfCardInGame(false, i);
                return;
            }
        }
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

    public void SavedCurrentDeckName()
    {
        LocalSaveManager.instance.user.monsterDeckName[indexCurrentDeckModifying] = nameOfCurrentDeckModifying.text;
        LocalSaveManager.instance.SaveUserData();
    }
    public void RemoveFromDeck(int index)
    {
        EnableDisableCardInListOfCardInGame(true, currentDeckModifying[index]);
        currentDeckModifying[index] = -1;
        listDisplayedCurrentDeckModifying[index].SetActive(false);
        totalMonsterInCurrentDeckModifying--;
        LocalSaveManager.instance.SaveUserData();
    }

    void ResetDisplayCurrentCardsDeck()
    {
        for (int i = 0; i < listDisplayedCurrentDeckModifying.Count; i++)
        {
            if (!currentDeckModifying[i].Equals(-1))
            {
                listDisplayedCurrentDeckModifying[i].GetComponent<Image>().sprite = null;
                listDisplayedCurrentDeckModifying[i].SetActive(false);
            }
        }
    }

    void DisplayCurrentCardsDeck()
    {
        totalMonsterInCurrentDeckModifying = 0;
        for (int i = 0; i < currentDeckModifying.Length; i++)
        {
            if (!currentDeckModifying[i].Equals(-1))
            {
                listDisplayedCurrentDeckModifying[i].GetComponent<Image>().sprite = LocalSaveManager.instance
                    .unitListScriptable.cards[i].miniaCard.GetComponent<Image>().sprite;
                listDisplayedCurrentDeckModifying[i].SetActive(true);
                totalMonsterInCurrentDeckModifying++;
            }
        }

        totalTextMonsterInCurrentDeckModifying.text = totalMonsterInCurrentDeckModifying + "/8";
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
                listDisplayedCardsDeck[i].GetComponent<DeckDisplay>().UpdateCounterCard();
                listDisplayedCardsDeck[i].GetComponent<DeckDisplay>().UpdateDeckName(LocalSaveManager.instance.user.monsterDeckName[i]);
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
            DisplayCardsDecks();
            ResetDisplayCurrentCardsDeck();
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

            nameOfCurrentDeckModifying.text = LocalSaveManager.instance.user.monsterDeckName[currentDeckIndex];
        }
        else
        {
            currentDeckModifying = listDicesDeck[currentDeckIndex];
        }

        page2And3.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear);
        
        DisplayElementForP2AndP3(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
