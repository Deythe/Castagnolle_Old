using System;
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
    [SerializeField] private GameObject bigCart;

    [SerializeField] private GameObject card;
    [SerializeField] private RectTransform cardListChose;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject settingsButtonMenu;
    [SerializeField] private GameObject viewButton;
    [SerializeField] private GameObject shader;
    
    private bool settingsOnOff;
    private bool viewTacticsOn;
    private float originalScrolPositionY;
    private float framerate;
    private float deltaTime;

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
        originalScrolPositionY = scrollView.transform.localPosition.y;
    }

    void Update()
    {
        ChangePosition();
        EnableDisableThrowDiceButton();
        EnableDisableScrollView();
        EnableDisableEndTurn();
        EnableDisableBattleButton();
        EnableDisableMenuYesChoice();
        EnableDisableMenuNoChoice();
        EnableDisableShader();
        UpdateFPS();
    }

    public void UpdateHp()
    {
        hp.text = "PV : " + LifeManager.instance.OwnLife;
    }
    
    public void UpdateHpEnnemi()
    {
        hpEnnemi.text = "PV : " + LifeManager.instance.EnnemiLife;
    }
    
    void UpdateFPS()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        framerate = 1.0f / deltaTime;
        fps.text = Mathf.Ceil (framerate).ToString ();
    }

    public void AbleUpdateCard(Sprite card)
    {
        bigCart.GetComponent<Image>().sprite = card;
        bigCart.SetActive(true);
    }

    public void ChangePosition()
    {
        if (card != null)
        {
            if(Input.touchCount > 0)
            {
                card.GetComponent<RectTransform>().localPosition = new Vector3(card.GetComponent<RectTransform>().localPosition.x,Input.touches[0].position.y-150,card.GetComponent<RectTransform>().localPosition.z);
            }
        }
    }

    public void ShowingOffBigCard()
    {
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
            scrollView.GetComponent<RectTransform>().DOLocalMoveY(originalScrolPositionY+300, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            scrollView.GetComponent<RectTransform>().DOLocalMoveY(originalScrolPositionY, 0.5f).SetEase(Ease.Linear);
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
    
    void EnableDisableShader()
    {
        if (RoundManager.instance.LocalPlayerTurn ==
            RoundManager.instance.CurrentPlayerNumberTurn && RoundManager.instance.StateRound!=2)
        {
            shader.SetActive(true);
        }
        else
        {
            shader.SetActive(false);
        }
    }
    
    
    
    
}
