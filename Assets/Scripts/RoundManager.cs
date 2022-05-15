using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviourPunCallbacks
{
    public static RoundManager instance;
    
    [SerializeField] private GameObject playerPref;
    [SerializeField] private PhotonView playerView;
    
    private int roundState; 
    private GameObject playerInstance;
    
    public int StateRound
    {
        get => roundState;
        set
        {
            roundState = value;
            UiManager.instance.ChangeRoundUI();
        }
    }

    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        roundState = 0;
        SpawnNewPlayer();
    }
    
    public void SpawnNewPlayer()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] == 1)
        {
            playerInstance =
                PhotonNetwork.Instantiate(playerPref.name, Vector3.zero, Quaternion.identity, 0);
        }
        else
        {
            playerInstance =
                PhotonNetwork.Instantiate(playerPref.name, Vector3.zero, new Quaternion(0,180,0,0), 0);
        }
        
        playerInstance.GetComponent<PlayerSetup>().enabled = true;
    }
    
    public void EndRound()
    {
        roundState = 0;
        PlacementManager.instance.ReInitMonster();
        BattlePhaseManager.instance.ClearUnits();

        playerView.RPC("RPC_EndTurn", RpcTarget.All);
    }

    public void BattlePhase()
    {
        roundState = 3;
    }
    
    public void Action()
    {
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
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] == 1)
        {
            PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] = 2;
        }
        else
        {
            PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] = 1;
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
}
