using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Image = UnityEngine.UI.Image;


public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [SerializeField] private GameObject waiting;
    [SerializeField] private Canvas canvas;

    [SerializeField] private TMP_Text fps;
    [SerializeField] private TMP_Text timer;
    
    [SerializeField] private TMP_Text player1FaceHp;
    [SerializeField] private TMP_Text player1ProfileHp;
    
    [SerializeField] private TMP_Text player2FaceHp;
    [SerializeField] private TMP_Text player2ProfileHp;
    
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

    [SerializeField] private Material lifeShaderAlly;
    [SerializeField] private Material lifeShaderEnemy;
    
    [SerializeField] private GameObject winSprite;
    [SerializeField] private GameObject looseSprite;

    [SerializeField] private List<Sprite> textFeedbacksList;
    [SerializeField] private Image textFeedBack;

    [SerializeField] private GameObject prefabHitMarker;
    [SerializeField] private List<GameObject> listHitMarkes;

    [SerializeField] private GameObject prefabEnemyPointer;
    [SerializeField] private GameObject instanceEnemyPointer;

    [SerializeField] private Animation banner;

    [SerializeField] private GameObject borderStatus;
    [SerializeField] private Image helpPage;
    
    private List<int> pivotResources;
    private bool settingsOnOff;
    private bool viewTacticsOn;
    private float originalScrollPositionY;
    private float framerate;
    private float deltaTime;
    private bool waitingCoroutine;
    private RaycastHit hit;
    private WaitForSeconds time = new WaitForSeconds(3f);

    public GameObject p_borderStatus
    {
        get => borderStatus;
    }
    public GameObject p_instanceEnemyPointer
    {
        get => instanceEnemyPointer;
    }

    public Image p_textFeedBack
    {
        get => textFeedBack;
    }

    public GameObject p_winSprite
    {
        get => winSprite;
    }
    
    public GameObject p_looseSprite
    {
        get => looseSprite;
    }

    public GameObject p_settingMenu
    {
        get => settingsMenu;
    }
    
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

    public void InitPlayerMarkers()
    {
        listHitMarkes = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            listHitMarkes.Add(Instantiate(prefabHitMarker, Vector3.zero, Quaternion.Euler(45,PlayerSetup.instance.transform.rotation.eulerAngles.y,0)));
            listHitMarkes[i].SetActive(false);
        }
        
        instanceEnemyPointer = Instantiate(prefabEnemyPointer,Vector3.zero, Quaternion.Euler(45,PlayerSetup.instance.transform.rotation.eulerAngles.y,0));
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
                            if(hit.collider != null && (hit.collider.GetComponent<Monster>() != null || hit.collider.GetComponent<UnitExtension>() != null))
                            {
                                StopCoroutine(CoroutineShowingcard(hit.collider));
                                StartCoroutine(CoroutineShowingcard(hit.collider));
                            }
                            break;
                        case TouchPhase.Ended:
                            StopCoroutine(CoroutineShowingcard(hit.collider));
                            ShowingOffBigCard();
                            break;
                    }
                }
            }
        }

    public void SetTextFeedBack(int i)
    {
        textFeedBack.sprite = textFeedbacksList[i];
    }

    private IEnumerator CoroutineShowingcard(Collider go)
    {
        yield return new WaitForSeconds(0.5f);
        if (Input.touchCount > 0)
        {
            if (go != null)
            {
                AbleBoardMonsterCard(go);
            }
        }
    }

    public void UpdateTimer()
    {
        timer.text = "" + RoundManager.instance.p_Timer;
        if (RoundManager.instance.p_Timer <= 10)
        {
            timer.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => timer.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
        }
    }

    public void UpdateHp()
    {
        if (RoundManager.instance.LocalPlayerTurn == 1)
        {
            player1FaceHp.text = "" + LifeManager.instance.OwnLife;
            player1ProfileHp.text = "" + LifeManager.instance.OwnLife;
        }
        else
        {
            player2FaceHp.text = "" + LifeManager.instance.OwnLife;
            player2ProfileHp.text = "" + LifeManager.instance.OwnLife;
        }
    }

    public void EnableDisableTimer(bool b)
    {
        timer.enabled = b;
    }
    public void UpdateHpEnnemi()
    {
        if (RoundManager.instance.LocalPlayerTurn == 1)
        {
            player2FaceHp.text = "" + LifeManager.instance.EnnemiLife;
            player2ProfileHp.text = "" + LifeManager.instance.EnnemiLife;
        }
        else
        {
            player1FaceHp.text = "" + LifeManager.instance.EnnemiLife;
            player1ProfileHp.text = "" + LifeManager.instance.EnnemiLife;
        }
    }
    
    void UpdateFPS()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        framerate = 1.0f / deltaTime;
        fps.text = Mathf.Ceil (framerate).ToString ();
    }
    
    public void AbleBoardMonsterCard(Collider go)
    {
        if (go.GetComponent<Monster>() != null)
        {
            bigCart.GetComponent<Image>().sprite = go.GetComponent<Monster>().BigCard;
            lifeCard.text = "" + go.GetComponent<Monster>().Atk;
            if (go.GetComponent<Monster>().Stats.GetComponent<CardData>() != null)
            {
                pivotResources = go.GetComponent<Monster>().Stats.GetComponent<CardData>().Ressources;
            }
        }
        else
        {
            bigCart.GetComponent<Image>().sprite = go.GetComponent<UnitExtension>().p_unitParent.GetComponent<Monster>().BigCard;
            lifeCard.text = "" + go.GetComponent<UnitExtension>().p_unitParent.GetComponent<Monster>().Atk;
            if (go.GetComponent<UnitExtension>().p_unitParent.GetComponent<Monster>().Stats.GetComponent<CardData>() !=
                null)
            {
                pivotResources = go.GetComponent<UnitExtension>().p_unitParent.GetComponent<Monster>().Stats
                    .GetComponent<CardData>().Ressources;
            }
        }

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
            ressourceCard.GetChild(i).GetComponent<Image>().sprite = DiceManager.instance.DiceListScriptable.symbolsList[card.GetComponent<CardData>().Ressources[i]];
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
            RoundManager.instance.CurrentPlayerNumberTurn && RoundManager.instance.StateRound==1 && DeckManager.instance.MonsterPossible.Count!=0)
        {
            scrollView.GetComponent<RectTransform>().DOLocalMoveY(originalScrollPositionY+240, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            scrollView.GetComponent<RectTransform>().DOLocalMoveY(originalScrollPositionY, 0.1f).SetEase(Ease.Linear);
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
        helpPage.enabled = false;
        settingsButtonMenu.SetActive(!settingsOnOff);
        settingsMenu.SetActive(settingsOnOff);
        viewButton.SetActive(!settingsOnOff);
        scrollView.SetActive(!settingsOnOff);
    }
    
    public void EnableDisableHelpPage()
    {
        helpPage.enabled = !helpPage.enabled;
    }
    
    public void EnableDisableShader(bool b)
    {
        shader.SetActive(b);
    }
    
    public void UpdateLifeShaderAlly(int value)
    {
        if (RoundManager.instance.LocalPlayerTurn == 1)
        {
            lifeShaderAlly.SetFloat("_Fill", value / 20f);
        }else
        {
            lifeShaderEnemy.SetFloat("_Fill", value / 20f);
        }
    }
    
    public void UpdateLifeShaderEnemy(int value)
    {
        if (RoundManager.instance.LocalPlayerTurn == 1)
        {
            lifeShaderEnemy.SetFloat("_Fill", value / 20f);
        }
        else
        {
            lifeShaderAlly.SetFloat("_Fill", value / 20f);
        }
    }

    public void ShowTextFeedBackWithDelay(int index)
    {
        StartCoroutine(CoroutineShowTextFeedBackWithDelay(index));
    }

    public void PlayHitMarker(Vector3 v, int damage)
    {
        foreach (GameObject hitMarker in listHitMarkes)
        {
            if (!hitMarker.GetComponent<GhostNumber>().p_isPlaying)
            {
                hitMarker.GetComponent<TMP_Text>().text = "-" + damage;
                hitMarker.transform.position = v + (2*Vector3.up);
                hitMarker.SetActive(true);
                return;
            }
        }
    }

    IEnumerator CoroutineShowTextFeedBackWithDelay(int index)
    {
        textFeedBack.sprite = textFeedbacksList[index];
        textFeedBack.enabled = true;
        yield return time;
        textFeedBack.enabled = false;
    }

    public void BannerItsYourTurnToPlay()
    {
        banner.Play();
    }


    public void EnableBorderStatus(float r, float g, float b)
    {
        borderStatus.GetComponent<Image>().color = new Color(r/255, g/255, b/255, 0);
        borderStatus.GetComponent<Image>().DOFade(1, 1f);
    }

    public void DisableBorderStatus()
    {
        borderStatus.GetComponent<Image>().DOFade(0, 1f);
    }

    public void BorderSingleFlash(float r, float g, float b)
    {
        borderStatus.GetComponent<Image>().color = new Color(r/255, g/255, b/255, 0);
        borderStatus.GetComponent<Image>().DOFade(1, 1f).OnComplete(DisableBorderStatus);
    }
    

}
