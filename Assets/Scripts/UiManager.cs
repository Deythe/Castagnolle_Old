using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;


public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [SerializeField] private GameObject waiting;
    [SerializeField] private Canvas canvas;

    [SerializeField] private TMP_Text fps;
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text hpEnnemi;
    
    [SerializeField] private GameObject uiPlayerTurn;
    [SerializeField] private GameObject endTurn;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject menuBattlePhase;
    
    [SerializeField] private GameObject menuYesChoice;
    [SerializeField] private GameObject menuNoChoice;
    
    [SerializeField] private RectTransform content;
    
    private GameObject card;
    
    [SerializeField] private GameObject bigCart;
    [SerializeField] private TMP_Text lifeCard;
    [SerializeField] private RectTransform ressourceCard;
    
    [SerializeField] private RectTransform cardListChose;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject settingsButtonMenu;
    [SerializeField] private GameObject viewButton;
    [SerializeField] private GameObject shader;


    private List<int> pivotResources;
    private bool settingsOnOff;
    private bool viewTacticsOn;
    private float originalScrollPositionY;
    private float framerate;
    private float deltaTime;
    private bool waitingCoroutine;
    private RaycastHit hit;
    public GameObject Waiting
    {
        get => waiting;
    }
    public bool ViewTacticsOn
    {
        get => viewTacticsOn;
    }

    public Canvas CanvasPublic
    {
        get => canvas;
        set
        {
            canvas = value;
        }
    }

    public RectTransform CarListChose
    {
        get => cardListChose;
    }
    public GameObject Card
    {
        get => card;
        set
        {
            card = value;
        }
    }
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        originalScrollPositionY = scrollView.transform.localPosition.y;
        
        if (RoundManager.instance.LocalPlayerTurn.Equals(1))
        {
            shader.SetActive(true);
        }
        else
        {
            shader.SetActive(false);
        }
    }

    void Update()
    {
        if (RoundManager.instance != null)
        {
            CheckRaycast();
            ChangePosition();
            EnableDisableThrowDiceButton();
            EnableDisableScrollView();
            EnableDisableEndTurn();
            EnableDisableBattleButton();
            EnableDisableMenuYesChoice();
            EnableDisableMenuNoChoice();
            UpdateFPS();
        }
    }
    
    void CheckRaycast()
        {
            if (RoundManager.instance.StateRound!=2)
            {
                if (Input.touchCount > 0)
                {
                    switch (Input.GetTouch(0).phase)
                    {
                        case TouchPhase.Began:
                            Physics.Raycast(PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position), out hit);
                            if(hit.collider != null && hit.collider.GetComponent<Monster>() != null)
                            {
                                StopAllCoroutines();
                                StartCoroutine(CoroutineShowingcard(hit.collider));
                            }
                            break;
                        case TouchPhase.Ended:
                            StopAllCoroutines();
                            ShowingOffBigCard();
                            break;
                    }
                }
            }
        }

    private IEnumerator CoroutineShowingcard(Collider go)
    {
        yield return new WaitForSeconds(0.3f);
        if (go != null)
        {
            AbleBoardMonsterCard(go);
        }
    }
    

    public void UpdateHp()
    {
        hp.text = ""+LifeManager.instance.OwnLife;
    }
    
    public void UpdateHpEnnemi()
    {
        hpEnnemi.text = ""+LifeManager.instance.EnnemiLife;
    }
    
    void UpdateFPS()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        framerate = 1.0f / deltaTime;
        fps.text = Mathf.Ceil (framerate).ToString ();
    }
    
    public void AbleBoardMonsterCard(Collider go)
    {
        bigCart.GetComponent<Image>().sprite = go.GetComponent<Monster>().BigCard;
        lifeCard.text = ""+ go.GetComponent<Monster>().Atk;
        pivotResources = go.GetComponent<Monster>().Stats.GetComponent<CardData>().Ressources;
        
        for (int i = 0; i <  pivotResources.Count; i++)
        {
            ressourceCard.GetChild(i).GetComponent<Image>().sprite =
                DiceManager.instance.DiceListScriptable.symbolsList[pivotResources[i]];
            ressourceCard.GetChild(i).gameObject.SetActive(true);
        }
        
        bigCart.SetActive(true);
    }

    public void AbleDeckCardTouch()
    {
        bigCart.GetComponent<Image>().sprite = card.GetComponent<CardData>().BigCard;
        lifeCard.text = ""+card.GetComponent<CardData>().Atk;

        for (int i = 0; i < card.GetComponent<CardData>().Ressources.Count; i++)
        {
            ressourceCard.GetChild(i).GetComponent<Image>().sprite =
                DiceManager.instance.DiceListScriptable.symbolsList[card.GetComponent<CardData>().Ressources[i]];
            ressourceCard.GetChild(i).gameObject.SetActive(true);
        }
        
        bigCart.SetActive(true);
    }

    public void ChangePosition()
    {
        if (card != null && !card.GetComponent<CardData>().IsTouching)
        {
            if(Input.touchCount > 0)
            {
                card.GetComponent<RectTransform>().localPosition = new Vector3(card.GetComponent<RectTransform>().localPosition.x,Input.touches[0].position.y-150,card.GetComponent<RectTransform>().localPosition.z);
            }
        }
    }

    public void ShowingOffBigCard()
    {
        for (int i = 0; i < ressourceCard.childCount; i++)
        {
            ressourceCard.GetChild(i).gameObject.SetActive(false);
        }
        
        bigCart.SetActive(false);
    }

    public void UpdateListCard()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            content.GetChild(i).gameObject.SetActive(false);
        }

        foreach (var unit in DeckManager.instance.MonsterPossible)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                if (content.GetChild(i).gameObject == unit.gameObject)
                {
                    content.GetChild(i).gameObject.SetActive(true);    
                }
            }
        }
    }

    public GameObject InitCard(GameObject go)
    {
        return Instantiate(go,content);
    }

    void EnableDisableThrowDiceButton()
    {
        if (RoundManager.instance.LocalPlayerTurn ==
            RoundManager.instance.CurrentPlayerNumberTurn && RoundManager.instance.StateRound==0)
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
        if (RoundManager.instance.LocalPlayerTurn ==
            RoundManager.instance.CurrentPlayerNumberTurn && RoundManager.instance.StateRound==1 && !DiceManager.instance.DeckEmpy())
        {
            scrollView.GetComponent<RectTransform>().DOLocalMoveY(originalScrollPositionY+250, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            scrollView.GetComponent<RectTransform>().DOLocalMoveY(originalScrollPositionY, 0.5f).SetEase(Ease.Linear);
        }
    }
    
    void EnableDisableMenuYesChoice()
    {
        if (RoundManager.instance.LocalPlayerTurn ==
            RoundManager.instance.CurrentPlayerNumberTurn && ((RoundManager.instance.StateRound==4 && BattlePhaseManager.instance.IsAttacking) || RoundManager.instance.StateRound==5))
        {
            menuYesChoice.GetComponent<RectTransform>().DOLocalMoveX(230, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            menuYesChoice.GetComponent<RectTransform>().DOLocalMoveX(550, 0.5f).SetEase(Ease.Linear);
        }
    }
    
    void EnableDisableMenuNoChoice()
    {
        if (RoundManager.instance.LocalPlayerTurn ==
            RoundManager.instance.CurrentPlayerNumberTurn && (RoundManager.instance.StateRound==4 || RoundManager.instance.StateRound==5 || RoundManager.instance.StateRound==6 || RoundManager.instance.StateRound==7))
        {
            menuNoChoice.GetComponent<RectTransform>().DOLocalMoveX(-230, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            
            menuNoChoice.GetComponent<Transform>().DOLocalMoveX(-550, 0.5f).SetEase(Ease.Linear);
        }
    }
    void EnableDisableBattleButton()
    {
        if (RoundManager.instance.LocalPlayerTurn ==
            RoundManager.instance.CurrentPlayerNumberTurn && RoundManager.instance.StateRound==1)
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
        if (RoundManager.instance.LocalPlayerTurn ==
            RoundManager.instance.CurrentPlayerNumberTurn && RoundManager.instance.StateRound==3)
        {
            endTurn.SetActive(true);
        }
        else
        {
            endTurn.SetActive(false);
        }
    }

    public void ChangeViewPlayer()
    {
        viewTacticsOn = !viewTacticsOn;
        PlayerSetup.instance.ChangeView(viewTacticsOn);
    }

    public void ChangeSettingStatus()
    {
        settingsOnOff = !settingsOnOff;
        settingsButtonMenu.SetActive(!settingsOnOff);
        settingsMenu.SetActive(settingsOnOff);
        viewButton.SetActive(!settingsOnOff);
        scrollView.SetActive(!settingsOnOff);
    }
    
    public void EnableDisableShader(bool b)
    {
        shader.SetActive(b);
    }
}
