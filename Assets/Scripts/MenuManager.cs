using System;
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
    [SerializeField] private TMP_Text searching;
    [SerializeField] private Button chooseDeckButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Transform allMenu;
    [SerializeField] private Transform content;
    private bool find;
    private Hashtable hash = new Hashtable();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        searching.text = "Connexion au serveur";
        chooseDeckButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
        playButton.interactable = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        nickname.text = FireBaseManager.instance.User.userName;
    }
    
    public Button PlayButton
    {
        get => playButton;
    }
    
    private void FixedUpdate()
    {
        if (find)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
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
            allMenu.DOLocalMoveX(-720, 0.2f).SetEase(Ease.Linear);
        }
        else
        {
            allMenu.DOLocalMoveX(0, 0.2f).SetEase(Ease.Linear);
        }
    }
    
    public void QuitQueue()
    {
        PhotonNetwork.Disconnect();
    }
    
    public void SearchGame()
    {
        PhotonNetwork.JoinRandomRoom();
        EnableDisableChoseDeck(false);
        DisableEnableAllDeckButton(false);
        playButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        chooseDeckButton.interactable = true;
        PhotonNetwork.JoinLobby();
    }
    

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        FireBaseManager.instance.User = null;
        SceneManager.LoadScene(0);
    }
    
    public override void OnJoinedLobby(){
        base.OnJoinedLobby();
        chooseDeckButton.interactable = true;
        searching.text = "Connecté";
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

        searching.text = "Recherche de joueurs";
    }

    public void GoToDeckScene()
    {
        SceneManager.LoadScene(3);
    }
}
