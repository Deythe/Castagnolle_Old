using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
    
    [SerializeField] private GameObject buttonThrowDice;
    [SerializeField] private GameObject buttonEndTurn;
    [SerializeField] private GameObject buttonBattlePhase;
    [SerializeField] private GameObject scrollView;

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
    [SerializeField] private GameObject helpPage;

    [SerializeField] private GameObject dicesPlayer1;
    [SerializeField] private GameObject dicesPlayer2;
    
    [SerializeField] private List<Sprite> buttonsSprite;
    [SerializeField] private Image musicButton;
    [SerializeField] private Image sfxButton;

    [SerializeField] private Button throwButton;
    [SerializeField] private CanvasScaler canvasScaler;

    private Color allyColor = new Color(0.058f,0.57f,0.66f, 1);
    private Color ennemyColor = new Color(0.84f,0.25f,0.15f,0.5f);

    private Coroutine currentCoroutine;
    private List<DiceListScriptable.enumRessources> pivotResources;
    private bool settingsOnOff;
    private bool viewTacticsOn;
    private float originalScrollPositionY;
    private float framerate;
    private float deltaTime;
    private bool waitingCoroutine;
    private RaycastHit hit;
    private WaitForSeconds time = new WaitForSeconds(3f);
    
    public Button p_throwButton
    {
        get => throwButton;
    }

    public GameObject p_dicePlayer1
    {
        get => dicesPlayer1;
    }
    
    public GameObject p_dicePlayer2
    {
        get => dicesPlayer2;
    }


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
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }

        bigCart.GetComponent<RectTransform>().localScale = new Vector3(canvasScaler.referenceResolution.y / 1920f, canvasScaler.referenceResolution.y / 1920f, canvasScaler.referenceResolution.y / 1920f);
        helpPage.GetComponent<RectTransform>().localScale = new Vector3(canvasScaler.referenceResolution.y / 1920f, canvasScaler.referenceResolution.y / 1920f, canvasScaler.referenceResolution.y / 1920f);
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        originalScrollPositionY = scrollView.transform.localPosition.y;
        
        if (RoundManager.instance.p_localPlayerTurn.Equals(1))
        {
            shader.SetActive(true);
        }
        else
        {
            shader.SetActive(false);
        }
        
        if (SoundManager.instance.p_musicEnabled)
        {
            musicButton.sprite = buttonsSprite[0];
        }
        else
        {
            musicButton.sprite = buttonsSprite[1];
        }
        
        if (SoundManager.instance.p_sfxEnabled)
        {
            sfxButton.sprite = buttonsSprite[2];
        }
        else
        {
            sfxButton.sprite = buttonsSprite[3];
        }
        
        DisableSomeHp();
    }
    void Update()
    {
        if (RoundManager.instance != null)
        {
            CheckRaycast();
            UpdateFPS();
        }
    }

    void CheckRaycast()
        {
            if (RoundManager.instance.p_roundState!= RoundManager.enumRoundState.DragUnitPhase)
            {
                if (Input.touchCount > 0)
                {
                    switch (Input.GetTouch(0).phase)
                    {
                        case TouchPhase.Began:
                            Physics.Raycast(PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position), out hit);
                            if(hit.collider != null && (hit.collider.GetComponent<Monster>() != null || hit.collider.GetComponent<UnitExtension>() != null))
                            {
                                if (currentCoroutine != null)
                                {
                                    StopCoroutine(currentCoroutine);
                                }
                                currentCoroutine = StartCoroutine(CoroutineShowingcard(hit.collider));
                            }
                            break;
                        
                        case TouchPhase.Ended:
                            if (currentCoroutine != null)
                            {
                                StopCoroutine(currentCoroutine);
                            }
                            ShowingOffBigCard();
                            break;
                    }
                }
            }
        }

    private void DisableSomeHp()
    {
        if (RoundManager.instance.p_localPlayerTurn == 1)
        {
            player1ProfileHp.enabled = false;
            player2FaceHp.enabled = false;
        }
        else
        {
            player2ProfileHp.enabled = false;
            player1FaceHp.enabled = false;
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
        if (RoundManager.instance.p_localPlayerTurn == 1)
        {
            player1FaceHp.text = "" + LifeManager.instance.OwnLife;
            player1FaceHp.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => player1FaceHp.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
            player1ProfileHp.text = "" + LifeManager.instance.OwnLife;
            player1ProfileHp.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => player1ProfileHp.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
        }
        else
        {
            player2FaceHp.text = "" + LifeManager.instance.OwnLife;
            player2FaceHp.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => player2FaceHp.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
            player2ProfileHp.text = "" + LifeManager.instance.OwnLife;
            player2ProfileHp.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => player2ProfileHp.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
        }
    }

    public void ChangeTimerColor(int i)
    {
        if (i == 1)
        {
            timer.color = allyColor;
        }
        else
        {
            timer.color = ennemyColor;
        }
    }
    
    public void UpdateHpEnnemi()
    {
        if (RoundManager.instance.p_localPlayerTurn == 1)
        {
            player2FaceHp.text = "" + LifeManager.instance.EnnemiLife;
            player2FaceHp.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => player2FaceHp.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
            player2ProfileHp.text = "" + LifeManager.instance.EnnemiLife;
            player2ProfileHp.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => player2ProfileHp.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
        }
        else
        {
            player1FaceHp.text = "" + LifeManager.instance.EnnemiLife;
            player1FaceHp.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => player1FaceHp.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
            player1ProfileHp.text = "" + LifeManager.instance.EnnemiLife;
            player1ProfileHp.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
                .OnComplete(() => player1ProfileHp.transform.DOScale(new Vector3(1, 1, 1), 0.5f));
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
            bigCart.GetComponent<Image>().sprite = go.GetComponent<Monster>().p_bigCard;
            lifeCard.text = "" + go.GetComponent<Monster>().p_atk;
            if (go.GetComponent<Monster>().p_stats.GetComponent<CardData>() != null)
            {
                pivotResources = go.GetComponent<Monster>().p_stats.GetComponent<CardData>().p_ressources;
            }
        }
        else
        {
            bigCart.GetComponent<Image>().sprite = go.GetComponent<UnitExtension>().p_unitParent.GetComponent<Monster>().p_bigCard;
            lifeCard.text = "" + go.GetComponent<UnitExtension>().p_unitParent.GetComponent<Monster>().p_atk;
            if (go.GetComponent<UnitExtension>().p_unitParent.GetComponent<Monster>().p_stats.GetComponent<CardData>() !=
                null)
            {
                pivotResources = go.GetComponent<UnitExtension>().p_unitParent.GetComponent<Monster>().p_stats
                    .GetComponent<CardData>().p_ressources;
            }
        }

        for (int i = 0; i <  pivotResources.Count; i++)
        {
            ressourceCard.GetChild(i).GetComponent<Image>().sprite = ChooseGoodSprite(pivotResources, i);
            ressourceCard.GetChild(i).gameObject.SetActive(true);
        }
        
        bigCart.SetActive(true);
    }
    
    public Sprite ChooseGoodSprite(List<DiceListScriptable.enumRessources> resources,int index)
    {
        switch (resources[index])
        {
            case DiceListScriptable.enumRessources.Whatever:
                return DiceManager.instance.DiceListScriptable.symbolsList[0];
            case DiceListScriptable.enumRessources.Blue:
                return DiceManager.instance.DiceListScriptable.symbolsList[1];
            case DiceListScriptable.enumRessources.Purple:
                return DiceManager.instance.DiceListScriptable.symbolsList[2];
            case DiceListScriptable.enumRessources.Red:
                return DiceManager.instance.DiceListScriptable.symbolsList[3];
        }

        return null;
    }

    public void AbleDeckCardTouch(Transform img)
    {
        bigCart.GetComponent<Image>().sprite = img.GetComponent<CardData>().BigCard;
        lifeCard.text = ""+img.GetComponent<CardData>().Atk;

        for (int i = 0; i < img.GetComponent<CardData>().p_ressources.Count; i++)
        {
            ressourceCard.GetChild(i).GetComponent<Image>().sprite =
                ChooseGoodSprite(img.GetComponent<CardData>().p_ressources, i);
            ressourceCard.GetChild(i).gameObject.SetActive(true);
        }
        
        bigCart.SetActive(true);
    }

    public void ShowingOffBigCard()
    {
        for (int i = 0; i < ressourceCard.childCount; i++)
        {
            ressourceCard.GetChild(i).gameObject.SetActive(false);
        }
        
        bigCart.SetActive(false);
    }

    public GameObject InitCard(GameObject go)
    {
        return Instantiate(go,content);
    }

    public void MoveCardAtEnd(GameObject card)
    {
        for (int i =0; i<content.childCount;i++ ) 
        {
            if (content.GetChild(i).gameObject.Equals(card))
            {
                content.GetChild(i).SetAsLastSibling();
            }
        }
    }

    public void EnableDisableThrowDiceButton(bool b)
    {
        if (b)
        {
            buttonThrowDice.SetActive(true);
        }
        else
        {
            buttonThrowDice.SetActive(false);
        }
    }
    
    public void EnableDisableScrollView(bool b)
    {
        if (b)
        {
            Debug.Log("Caca");
            scrollView.GetComponent<RectTransform>().DOLocalMoveY(originalScrollPositionY+(canvasScaler.referenceResolution.y/7f), 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            scrollView.GetComponent<RectTransform>().DOLocalMoveY(originalScrollPositionY, 0.1f).SetEase(Ease.Linear);
        }
    }
    
    public void EnableDisableMenuYesChoice(bool b)
    {
        if(b)
        {
            menuYesChoice.GetComponent<RectTransform>().DOLocalMoveX(Screen.width*0.3f, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            menuYesChoice.GetComponent<RectTransform>().DOLocalMoveX(Screen.width, 0.5f).SetEase(Ease.Linear);
        }
    }
    
    public void EnableDisableMenuNoChoice(bool b)
    {
        if(b){
            menuNoChoice.GetComponent<RectTransform>().DOLocalMoveX(Screen.width*-0.3f, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            
            menuNoChoice.GetComponent<Transform>().DOLocalMoveX(-Screen.width, 0.5f).SetEase(Ease.Linear);
        }
    }
    public void EnableDisableBattleButton(bool b)
    {
        if (b)
        {
            buttonBattlePhase.SetActive(true);
        }
        else
        {
            buttonBattlePhase.SetActive(false);
        }
    }
    
    public void EnableDisableEndTurn(bool b)
    {
        if (b)
        {
            buttonEndTurn.SetActive(true);
        }
        else
        {
            buttonEndTurn.SetActive(false);
        }
    }

    public void ChangeViewPlayer()
    {
        viewTacticsOn = !viewTacticsOn;
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        PlayerSetup.instance.ChangeView(viewTacticsOn);
    }

    public void ChangeSettingStatus()
    {
        settingsOnOff = !settingsOnOff;
        if (settingsOnOff)
        {
            SoundManager.instance.PlaySFXSound(0, 0.07f);
        }
        else
        {
            SoundManager.instance.PlaySFXSound(1, 0.07f);
        }
        helpPage.SetActive(false);
        settingsButtonMenu.SetActive(!settingsOnOff);
        settingsMenu.SetActive(settingsOnOff);
        viewButton.SetActive(!settingsOnOff);
        scrollView.SetActive(!settingsOnOff);
    }
    
    public void EnableDisableHelpPage()
    {
        helpPage.SetActive(!helpPage.activeSelf);
        SoundManager.instance.PlaySFXSound(0, 0.07f);
    }
    
    public void EnableDisableShader(bool b)
    {
        shader.SetActive(b);
    }
    
    public void UpdateLifeShaderAlly(int value)
    {
        if (RoundManager.instance.p_localPlayerTurn == 1)
        {
            lifeShaderAlly.SetFloat("_Fill", value / 20f);
        }else
        {
            lifeShaderEnemy.SetFloat("_Fill", value / 20f);
        }
    }
    
    public void UpdateLifeShaderEnemy(int value)
    {
        if (RoundManager.instance.p_localPlayerTurn == 1)
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
    
    public void EnableDisableSoundManagerMusic()
    {
        SoundManager.instance.EnableDisableMusic();
        if (SoundManager.instance.p_musicEnabled)
        {
            musicButton.sprite = buttonsSprite[0];
            SoundManager.instance.PlaySFXSound(0, 0.07f);
        }
        else
        {
            SoundManager.instance.PlaySFXSound(1, 0.07f);
            musicButton.sprite = buttonsSprite[1];
        }
    }
    
    public void EnableDisableSoundManagerSFX()
    {
        SoundManager.instance.EnableDisableSFX();
        if (SoundManager.instance.p_sfxEnabled)
        {
            sfxButton.sprite = buttonsSprite[2];
            SoundManager.instance.PlaySFXSound(0, 0.07f);
        }
        else
        {
            sfxButton.sprite = buttonsSprite[3];
        }
    }
}
