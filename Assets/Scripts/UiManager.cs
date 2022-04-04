using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;


public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    [SerializeField] private TMP_Text numberRound;
   
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
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        ChangeRoundUI();
        ChangePosition();
        
        EnableDisableThrowDiceButton();
        EnableDisableScrollView();
        EnableDisableEndTurn();
        EnableDisableBattleButton();
        EnableDisableMenuYesChoice();
        EnableDisableMenuNoChoice();
        
        UpdateHpEnnemi();
        UpdateFPS();
        UpdateHp();
    }

    void UpdateHp()
    {
        hp.text = "PV : " + LifeManager.instance.GetOwnLife();
    }
    
    void UpdateHpEnnemi()
    {
        hpEnnemi.text = "PV : " + LifeManager.instance.GetEnnemiLife();
    }
    
    void UpdateFPS()
    {
        fps.text = "" + 1 / Time.deltaTime;
    }

    public void AbleUpdateCard(Image card)
    {
        bigCart.GetComponent<Image>().sprite = card.sprite;
        bigCart.SetActive(true);
    }

    public void ChangePosition()
    {
        if (card != null)
        {
            if(Input.touchCount > 0)
            {
                card.GetComponent<RectTransform>().position = new Vector3(card.GetComponent<RectTransform>().position.x,Input.touches[0].position.y,card.GetComponent<RectTransform>().position.z);
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
            Destroy(content.GetChild(i).gameObject);
        }

        foreach (var unit in DeckManager.instance.GetMonsters())
        {
            Instantiate(unit,content);
        }
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
            scrollView.GetComponent<RectTransform>().DOMoveY(200, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            scrollView.GetComponent<RectTransform>().DOMoveY(-50, 0.5f).SetEase(Ease.Linear);
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
    void EnableDisableEndTurn()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] ==
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] && RoundManager.instance.GetStateRound()==3)
        {
            endTurn.SetActive(true);
        }
        else
        {
            endTurn.SetActive(false);
        }
    }
    void ChangeRoundUI()
    {
        numberRound.text = "" + (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"];
    }

    public void SetCard(GameObject obj)
    {
        card = obj;
    }

    public GameObject GetCard()
    {
        return card;
    }
    
    
}
