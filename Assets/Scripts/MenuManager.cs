using System;
using System.Collections.Generic;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager instance;
    
    [SerializeField] private TMP_Text nickname;
    
    [SerializeField] private Button chooseDeckButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Transform allMenu;
    [SerializeField] private Transform content;
    [SerializeField] private SpriteRenderer animRecherche;
    [SerializeField] private Image connexionStatus;
    [SerializeField] private List<Sprite> connexionSprites;
    [SerializeField] private GameObject transparency;
    [SerializeField] private GameObject tutoObject;
    [SerializeField] private Image shader;
    
    [SerializeField] private List<Sprite> buttonsSprite;
    [SerializeField] private Image musicButton;
    [SerializeField] private Image sfxButton;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private int numberPlayer;
    private bool find;
    private Hashtable hash = new Hashtable();
    private void Awake()
    {
        Time.timeScale = 1;
        instance = this;
    }

    private void Start()
    {
        ResetStaticVars();

        connexionStatus.sprite = connexionSprites[1];
        animRecherche.enabled = false;
        chooseDeckButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
        playButton.interactable = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        nickname.text = FireBaseManager.instance.User.userName;
        
        if (FireBaseManager.instance.User.firstTime)
        {
            transparency.SetActive(true);
            tutoObject.transform.DOScale(new Vector3(1.5f,1.5f,1.5f), 1f).SetEase(Ease.OutQuint);
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
    }

    public Image p_shader
    {
        get => shader;
    }
    public Button PlayButton
    {
        get => playButton;
    }
    
    private void FixedUpdate()
    {
        if (find)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == numberPlayer)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                SceneManager.LoadScene(2);
            }
        }
    }

    public void DisableEnableAllDeckButton(bool b)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            content.GetChild(i).GetComponent<Button>().interactable = b;
        }
    }

    public void EnableDisableChoseDeck(bool b)
    {
        if (b)
        {
            SoundManager.instance.PlaySFXSound(0, 0.07f);
            allMenu.DOLocalMoveX(-canvasScaler.referenceResolution.x, 0.2f).SetEase(Ease.Linear);
        }
        else
        {
            allMenu.DOLocalMoveX(0, 0.2f).SetEase(Ease.Linear);
        }
    }

    public void SearchGame()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        PhotonNetwork.JoinRandomRoom();
        EnableDisableChoseDeck(false);
        DisableEnableAllDeckButton(false);
        playButton.interactable = false;
        //shader.enabled = false;
    }

    public override void OnConnectedToMaster()
    {
        chooseDeckButton.interactable = true;
        connexionStatus.sprite = connexionSprites[1];
        PhotonNetwork.JoinLobby();
    }
    

    public void Disconnect()
    {
        SoundManager.instance.PlaySFXSound(1, 0.07f);
        PhotonNetwork.Disconnect();
        FireBaseManager.instance.User = null;
        SceneManager.LoadScene(0);
    }
    
    public override void OnJoinedLobby(){
        base.OnJoinedLobby();
        chooseDeckButton.interactable = true;
        connexionStatus.sprite = connexionSprites[2];
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Creation du lobby");
        RoomOptions room = new RoomOptions();
        room.IsOpen = true;
        room.MaxPlayers = 2;
        room.IsVisible = true;
        PhotonNetwork.CreateRoom(null, room, null);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("On joined Room");
        base.OnJoinedRoom();
        find = true;
        animRecherche.enabled = true;
    }
    
    public override void OnDisconnected (DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        if (connexionStatus != null)
        {
            connexionStatus.sprite = connexionSprites[0];
        }
        
        playButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void GoToTuto()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        SceneManager.LoadScene(4);
    }
    
    public void GoToDeckBuilding()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        SceneManager.LoadScene(3);
    }

    public void CancelTuto()
    {
        SoundManager.instance.PlaySFXSound(1, 0.07f);
        FireBaseManager.instance.User.firstTime = false;
        if (FireBaseManager.instance.User.isConnected)
        {
            FireBaseManager.instance.FirstTimeChecked();
        }
        transparency.SetActive(false);
        tutoObject.transform.DOScale(Vector3.zero, 0.75f).SetEase(Ease.InQuad);
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
    
    void ResetStaticVars()
    {
        NeedLessRessourceForUnit.motherUnit = null;
        NeedLessRessourceForUnit.unitOnBoard = null;
        InvokeHimselfWithStat.motherUnit = null;
        NeedLessRessourcesForChampions.motherUnit = null;
        NeedLessRessourcesForChampions.originalCard = null;
        NeedLessRessourcesForChampions.unitOnBoard = null;
        BattlePhaseManager.instance = null;
        DeckManager.instance = null;
        DiceManager.instance = null;
        EffectManager.instance = null;
        LifeManager.instance = null;
        PlacementManager.instance = null;
        UiManager.instance = null;
        RoundManager.instance = null;
    }
}
