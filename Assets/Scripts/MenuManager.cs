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
        EnableDisableInQueue();
        
        if (find)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
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

    void EnableDisableInQueue()
    {
        searching.enabled = find;
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
        EnableDisableChoseDeck(false);
        DisableEnableAllDeckButton(false);
        playButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        searching.text = "Connexion au serveur";
        PhotonNetwork.AuthValues.UserId = FireBaseManager.instance.UserFireBase.UserId;
        PhotonNetwork.JoinLobby();
    }

    public void Disconnect()
    {
        FireBaseManager.instance.User = null;
        SceneManager.LoadScene(0);
    }
    
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        searching.text = "Connecté au serveur";
        Debug.Log(PhotonNetwork.CountOfRooms);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Creation du lobby");
        RoomOptions room = new RoomOptions();
        room.IsOpen = true;
        room.MaxPlayers = 2;
        room.IsVisible = true;
        PhotonNetwork.CreateRoom(null, room, null);
        Debug.Log("testPROUT");
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
        /*
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            hash["PlayerNumber"] = 1;
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            hash["PlayerNumber"] = 2;
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
        
        hash["RoundNumber"] = 1;
        PhotonNetwork.LocalPlayer.CustomProperties = hash;
        */
        
        find = true;

        searching.text = "Recherche de joueurs";
    }

    public void GoToDeckScene()
    {
        SceneManager.LoadScene(3);
    }
}
