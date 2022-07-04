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
    
    [SerializeField] private GameObject playerPref;
    [SerializeField] private PhotonView playerView;
    [SerializeField] private int localPlayerTurn;
    [SerializeField] private int currentPlayerNumberTurnNumberTurn;
    [SerializeField] private int timerPerRound = 60;
    [SerializeField] private List<Material> listMatPlayerGlow;
    private int timer;
    private int roundState; 
    private GameObject playerInstance;
    private WaitForSeconds tick= new WaitForSeconds(1);

    public List<Material> p_listMatPlayerGlow
    {
        get => listMatPlayerGlow;
    }
    
    public int p_Timer
    {
        get => timer;
    }
    public int LocalPlayerTurn
    {
        get => localPlayerTurn;
    }

    public int CurrentPlayerNumberTurn
    {
        get => currentPlayerNumberTurnNumberTurn;
    }
    
    public int StateRound
    {
        get => roundState;
        set
        {
            roundState = value;
            switch (roundState)
            {
                case 0:
                    StartCoroutine(CoroutineTimer());
                    break;
                case 1:
                    UiManager.instance.DisableBorderStatus();
                    UiManager.instance.p_instanceEnemyPointer.SetActive(false);
                    UiManager.instance.p_textFeedBack.enabled = false;
                    UiManager.instance.DisableBorderStatus();
                    break;
                case 3:
                    UiManager.instance.DisableBorderStatus();
                    UiManager.instance.p_instanceEnemyPointer.SetActive(false);
                    UiManager.instance.p_textFeedBack.enabled = false;
                    UiManager.instance.EnableBorderStatus(255,0,0);
                    break;
                case 4 :
                    UiManager.instance.SetTextFeedBack(4);
                    UiManager.instance.p_textFeedBack.enabled = true;
                    break;
                case 5:
                    UiManager.instance.EnableBorderStatus(68,168,254);
                    break;
                case 6:
                    UiManager.instance.EnableBorderStatus(68,168,254);
                    UiManager.instance.p_instanceEnemyPointer.SetActive(false);
                    UiManager.instance.SetTextFeedBack(2);
                    UiManager.instance.p_textFeedBack.enabled = true;
                    break;
                case 7 :
                    UiManager.instance.EnableBorderStatus(68,168,254);
                    UiManager.instance.p_instanceEnemyPointer.SetActive(false);
                    UiManager.instance.SetTextFeedBack(1);
                    UiManager.instance.p_textFeedBack.enabled = true;
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
        currentPlayerNumberTurnNumberTurn = 1;
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
        roundState = 0;
        SpawnNewPlayer();
    }

    IEnumerator CoroutineTimer()
    {
        UiManager.instance.UpdateTimer();
        yield return tick;
        timer--;
        if (timer < 0)
        {
            EndRound();
        }
        else
        {
            StartCoroutine(CoroutineTimer());
        }
    }

    public void SpawnNewPlayer()
    {
        if (localPlayerTurn==1)
        {
            playerInstance =
                PhotonNetwork.Instantiate(playerPref.name, Vector3.zero, Quaternion.identity, 0);
            timer = timerPerRound;
            StopAllCoroutines();
            UiManager.instance.EnableDisableTimer(true);
            StartCoroutine(CoroutineTimer());
            UiManager.instance.BannerItsYourTurnToPlay();
        }
        else
        {
            playerInstance =
                PhotonNetwork.Instantiate(playerPref.name, Vector3.zero, new Quaternion(0,180,0,0), 0);
            UiManager.instance.EnableDisableTimer(false);
        }
        
        playerInstance.GetComponent<PlayerSetup>().enabled = true;
        
    }
    
    public void EndRound()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        StopAllCoroutines();

        if (!PlacementManager.instance.IsWaiting)
        {
            PlacementManager.instance.ReInitPlacement();
        }
        
        UiManager.instance.DisableBorderStatus();
        UiManager.instance.p_instanceEnemyPointer.SetActive(false);
        DiceManager.instance.DeleteAllResources(DiceManager.instance.DiceChoosen);
        UiManager.instance.p_textFeedBack.enabled = false;
        PlacementManager.instance.ReInitMonster();
        BattlePhaseManager.instance.ClearUnits();
        EffectManager.instance.Cancel();
        UiManager.instance.p_throwButton.interactable = true;
        
        playerView.RPC("RPC_EndTurn", RpcTarget.All);
    }

    public void BattlePhase()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        SoundManager.instance.PlaySFXSound(6, 0.05f);
        DiceManager.instance.DeleteAllResources(DiceManager.instance.DiceChoosen);
        StateRound = 3;
    }
    
    public void Action()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        switch (roundState)
        {
            case 4:
                BattlePhaseManager.instance.Attack();
                break;
            case 5:
                EffectManager.instance.Action();
                break;
        }
    }

    public void CancelAction()
    {
        SoundManager.instance.PlaySFXSound(1, 0.07f);
        switch (roundState)
        {
            case 4:
                BattlePhaseManager.instance.CancelSelection();
                break;
            case 5:
            case 6:
            case 7:
                EffectManager.instance.CancelSelection(1);
                break;
        }
    }
    
    [PunRPC]
    private void RPC_EndTurn()
    {
        if (currentPlayerNumberTurnNumberTurn == 1)
        {
            currentPlayerNumberTurnNumberTurn = 2;
        }
        else
        {
            currentPlayerNumberTurnNumberTurn = 1;
        }

        if (currentPlayerNumberTurnNumberTurn.Equals(localPlayerTurn))
        {
            timer = timerPerRound;
            UiManager.instance.BannerItsYourTurnToPlay();
            UiManager.instance.EnableDisableTimer(true);
            UiManager.instance.EnableDisableShader(true);
            StateRound = 0;
        }
        else
        {
            UiManager.instance.EnableDisableShader(false);
            UiManager.instance.EnableDisableTimer(false);
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
