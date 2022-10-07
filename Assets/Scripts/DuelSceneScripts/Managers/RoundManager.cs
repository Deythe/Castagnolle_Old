using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class RoundManager : MonoBehaviourPunCallbacks
{
    public static RoundManager instance;
    
    public enum enumRoundState
    {
        ThrowPhase,DrawPhase,DragUnitPhase,CastagnePhase,EffectPhase,WaitPhase
    }
    
    [SerializeField] private GameObject playerPref;
    [SerializeField] private PhotonView playerView;
    [SerializeField] private int localPlayerTurn;
    [SerializeField] private int currentPlayerNumberTurn;
    [SerializeField] private int timerPerRound = 60;
    [SerializeField] private List<Material> listMatPlayerGlow;
    [SerializeField] private Transform board;
    
    private int timer;
    private enumRoundState roundState;
    private GameObject playerInstance;
    private WaitForSeconds tick= new WaitForSeconds(1);
    private Quaternion rotate = new Quaternion(0,180,0,0);

    public List<Material> p_listMatPlayerGlow
    {
        get => listMatPlayerGlow;
    }

    public int p_currentPlayerNumberTurn
    {
        get => currentPlayerNumberTurn;
        set
        {
            StopAllCoroutines();
            currentPlayerNumberTurn = value;
            
            if (currentPlayerNumberTurn == localPlayerTurn)
            {
                UiManager.instance.BannerItsYourTurnToPlay();
                UiManager.instance.ChangeTimerColor(1);
                UiManager.instance.EnableDisableShader(true);
                p_roundState = 0;
            }
            else
            {
                
                UiManager.instance.EnableDisableShader(false);
                UiManager.instance.ChangeTimerColor(2);
            }
        
            p_Timer = timerPerRound;
        }
    }
    
    public int p_Timer
    {
        get => timer;
        set
        {
            timer = value;
            if (timer < 0)
            {
                timer = 0;
                if(p_localPlayerTurn.Equals(currentPlayerNumberTurn))
                {
                    EndRound();
                }
            }
            else
            {
                StartCoroutine(CoroutineTimer());
            }
        }
    }
    public int p_localPlayerTurn
    {
        get => localPlayerTurn;
    }

    public int CurrentPlayerNumberTurn
    {
        get => currentPlayerNumberTurn;
    }
    
    public enumRoundState p_roundState
    {
        get => roundState;
        set
        {
            roundState = value;
            switch (roundState)
            {
                case enumRoundState.ThrowPhase:
                    UiManager.instance.EnableDisableBattleButton(false);
                    UiManager.instance.EnableDisableEndTurn(false);
                    UiManager.instance.EnableDisableThrowDiceButton(true);
                    break;
                case enumRoundState.DrawPhase:
                    UiManager.instance.EnableDisableThrowDiceButton(false);
                    UiManager.instance.EnableDisableBattleButton(true);
                    UiManager.instance.EnableDisableScrollView(true);
                    UiManager.instance.DisableBorderStatus();
                    UiManager.instance.p_instanceEnemyPointer.SetActive(false);
                    UiManager.instance.p_textFeedBack.enabled = false;
                    break;
                case enumRoundState.DragUnitPhase:
                    UiManager.instance.EnableDisableScrollView(false);
                    UiManager.instance.EnableDisableBattleButton(false);
                    break;
                case enumRoundState.CastagnePhase:
                    UiManager.instance.EnableDisableEndTurn(true);
                    UiManager.instance.EnableDisableBattleButton(false);
                    UiManager.instance.EnableDisableScrollView(false);
                    UiManager.instance.DisableBorderStatus();
                    UiManager.instance.p_instanceEnemyPointer.SetActive(false);
                    UiManager.instance.p_textFeedBack.enabled = false;
                    UiManager.instance.EnableBorderStatus(255,0,0);
                    break;
                case enumRoundState.EffectPhase:
                    UiManager.instance.EnableDisableEndTurn(false);
                    UiManager.instance.EnableDisableBattleButton(false);
                    UiManager.instance.EnableDisableMenuNoChoice(true);
                    UiManager.instance.EnableDisableScrollView(false);
                    UiManager.instance.EnableBorderStatus(68,168,254);
                    break;
                case enumRoundState.WaitPhase :
                    EffectManager.instance.ClearUnits();
                    CastagneManager.instance.ClearUnits();
                    UiManager.instance.EnableDisableScrollView(false);
                    UiManager.instance.EnableDisableMenuYesChoice(false);
                    UiManager.instance.EnableDisableMenuNoChoice(false);
                    UiManager.instance.EnableDisableEndTurn(false);
                    UiManager.instance.EnableDisableBattleButton(false);
                    UiManager.instance.EnableDisableEndTurn(false);
                    UiManager.instance.EnableDisableThrowDiceButton(false);
                    UiManager.instance.DisableBorderStatus();
                    UiManager.instance.EnableDisableShader(false);
                    UiManager.instance.p_instanceEnemyPointer.SetActive(false);
                    break;
            }
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
        currentPlayerNumberTurn = 1;
        if (PhotonNetwork.IsMasterClient)
        {
            localPlayerTurn = 1;
        }
        else
        {
            localPlayerTurn = 2;
        }
        
    }

    void Start()
    {
        SoundManager.instance.StarGameMusic();
        SpawnNewPlayer();
    }

    IEnumerator CoroutineTimer()
    {
        UiManager.instance.UpdateTimer();
        yield return tick;
        p_Timer--;
    }

    public void SpawnNewPlayer()
    {
        if (localPlayerTurn==1)
        {
            playerInstance =
                PhotonNetwork.Instantiate(playerPref.name, Vector3.zero, Quaternion.identity, 0);
            StopAllCoroutines();
            p_roundState = 0;
            UiManager.instance.ChangeTimerColor(1);
            UiManager.instance.BannerItsYourTurnToPlay();
        }
        else
        {
            playerInstance =
                PhotonNetwork.Instantiate(playerPref.name, Vector3.zero, rotate, 0);
            UiManager.instance.ChangeTimerColor(2);
            board.Rotate(0,0,-180);
        }
        
        p_Timer = timerPerRound;
        playerInstance.GetComponent<PlayerSetup>().enabled = true;
        
    }
    
    public void EndRound()
    {
        StopAllCoroutines();
        p_roundState = enumRoundState.WaitPhase;
        StartCoroutine(CoroutineEndRound());
    }

    IEnumerator CoroutineEndRound()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);

        yield return new WaitUntil(() => !PlacementManager.instance.p_isWaiting);
        
        PlacementManager.instance.ReInitPlacement();
        DiceManager.instance.DeleteAllResources(DiceManager.instance.p_diceChoosen);
        UiManager.instance.p_textFeedBack.enabled = false;
        PlacementManager.instance.ReInitMonsters();
        CastagneManager.instance.ClearUnits();
        EffectManager.instance.ClearUnits();
        UiManager.instance.p_throwButton.interactable = true;
        playerView.RPC("RPC_EndTurn", RpcTarget.AllViaServer);
    }

    public void BattlePhase()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        SoundManager.instance.PlaySFXSound(6, 0.05f);
        p_roundState = enumRoundState.CastagnePhase;
    }
    
    public void Action()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        switch (roundState)
        {
            case enumRoundState.DrawPhase:
                EffectManager.instance.UnitSelected(EffectManager.enumEffectConditionActivation.WhenItsDrawPhase);
                break;
            case enumRoundState.EffectPhase:
                EffectManager.instance.Action();

                break;
            case enumRoundState.CastagnePhase:
                CastagneManager.instance.Attack();
                break;
        }
    }

    public void CancelAction()
    {
        UiManager.instance.EnableDisableMenuNoChoice(false);
        UiManager.instance.EnableDisableMenuYesChoice(false);
        
        SoundManager.instance.PlaySFXSound(1, 0.07f);
        switch (roundState)
        {
            case enumRoundState.CastagnePhase:
                CastagneManager.instance.CancelSelection();
                break;
            case enumRoundState.DrawPhase:
                UiManager.instance.EnableDisableScrollView(true);
                EffectManager.instance.CancelSelection();
                break;
            case enumRoundState.EffectPhase:
                EffectManager.instance.CancelSelection();
                break;
        }
    }
    
    [PunRPC]
    private void RPC_EndTurn()
    {
        if (p_currentPlayerNumberTurn == 1)
        {
            p_currentPlayerNumberTurn = 2;
        }
        else
        {
            p_currentPlayerNumberTurn = 1;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected (DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        SceneManager.LoadScene(1);
    }

    public void PlayDiceAnim()
    {
        UiManager.instance.p_throwButton.interactable = false;
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        StartCoroutine(CoroutinePlayDiceAnim());
    }

    IEnumerator CoroutinePlayDiceAnim()
    {
        SoundManager.instance.PlaySFXSound(4, 0.07f);
        if (localPlayerTurn == 1)
        {
            UiManager.instance.p_dicePlayer1.SetActive(true);
            UiManager.instance.p_dicePlayer2.SetActive(false);
            for (int i = 0; i < UiManager.instance.p_dicePlayer1.transform.childCount; i++)
            {
                UiManager.instance.p_dicePlayer1.transform.GetChild(i).GetComponent<Animation>().Play();
            }
        }
        else
        {
            UiManager.instance.p_dicePlayer2.SetActive(true);
            UiManager.instance.p_dicePlayer1.SetActive(false);
            for (int i = 0; i < UiManager.instance.p_dicePlayer2.transform.childCount; i++)
            {
                UiManager.instance.p_dicePlayer2.transform.GetChild(i).GetComponent<Animation>().Play();
            }
        }

        yield return tick;
        UiManager.instance.p_dicePlayer1.SetActive(false);
        UiManager.instance.p_dicePlayer2.SetActive(false);
        DiceManager.instance.ChooseDice();
    }
}
