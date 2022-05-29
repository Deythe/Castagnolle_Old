using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
     public static LifeManager instance;
     
     [SerializeField] private int life;
     [SerializeField] private int ennemiLife;
     [SerializeField] private PhotonView lifeManagerView;

     public PhotonView View
     {
          get => lifeManagerView;
     }

     public int EnnemiLife
     {
          get => ennemiLife;
          set
          {
               if (PlayerSetup.instance != null)
               {
                    UiManager.instance.PlayHitMarker(
                         new Vector3(-0.5f*Math.Sign(PlayerSetup.instance.p_startCamPos.z), 1f, -6.5f * Math.Sign(PlayerSetup.instance.p_startCamPos.z)), ennemiLife - value);
               }

               ennemiLife = value;
               UiManager.instance.UpdateHpEnnemi();
               UiManager.instance.UpdateLifeShaderEnemy(ennemiLife);
               CheckWinGame();
          }
     }
     
     public int OwnLife
     {
          get => life;
          set
          {
               if (PlayerSetup.instance != null)
               {
                    UiManager.instance.PlayHitMarker(
                         new Vector3(0, 1, 7f * Math.Sign(PlayerSetup.instance.p_startCamPos.z)), life - value);
               }
               
               if ((life - value).Equals(3))
               {
                    Debug.Log("Prout");
                    UiManager.instance.BorderSingleFlash(255,0,0);
               }

               life = value;
               UiManager.instance.UpdateHp();
               UiManager.instance.UpdateLifeShaderAlly(life);
               CheckLoseGame();
          }
     }
     
     private void Awake()
     {
          instance = this;
          OwnLife = 20;
          EnnemiLife = 20;
     }

     public void TakeDamageHimself()
     {
          OwnLife -= 1;
          lifeManagerView.RPC("RPC_TakeDamageHimself", RpcTarget.Others, 1);
     }


     public void TakeDamageEnnemi(float i)
     {
          int result=0;
          if (RoundManager.instance.LocalPlayerTurn==1)
          {
               if (i <= 1.5)
               {
                    result = 1;
               }
               else if (i.Equals(2.5f) || i.Equals(3.5f))
               {
                    result = 2;
               }
               else if(i.Equals(4.5f))
               {
                    result = 3;
               }
          }
          else
          {
               if (i >= -1.5)
               {
                    result = 1;
               }
               else if (i.Equals(-2.5f) || i.Equals(-3.5f))
               {
                    result = 2;
               }
               else if(i.Equals(-4.5f))
               {
                    result = 3;
               }
          }
          
          EnnemiLife -= result;
          lifeManagerView.RPC("RPC_TakeDamageEnnemi", RpcTarget.Others, result);
     }

     public void CheckWinGame()
     {
          if (ennemiLife <= 0)
          {
               StopAllCoroutines();
               StartCoroutine(CoroutineShowWinOrLose());
          }
     }

     public void CheckLoseGame()
     {
          if (life <= 0)
          {
               StopAllCoroutines();
               StartCoroutine(CoroutineShowWinOrLose());
          }
     }

     public void GiveUp()
     {
          lifeManagerView.RPC("RPC_GiveUp", RpcTarget.AllViaServer, RoundManager.instance.LocalPlayerTurn);
     }

     [PunRPC]
     private void RPC_GiveUp(int playRef)
     {
          if (RoundManager.instance.LocalPlayerTurn.Equals(playRef))
          {
               OwnLife = 0;
          }
          else
          {
               EnnemiLife = 0;
          }
     }

     IEnumerator CoroutineShowWinOrLose()
     {
          UiManager.instance.p_settingMenu.SetActive(false);
          if (life > 0 && ennemiLife<=0)
          {
               UiManager.instance.p_winSprite.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).OnComplete(()=>Time.timeScale = 0);
          }else if (life <= 0 && ennemiLife>0)
          {
               UiManager.instance.p_looseSprite.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).OnComplete(()=>Time.timeScale = 0);
          }else if (life <= 0 && ennemiLife<=0)
          {
               UiManager.instance.p_looseSprite.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).OnComplete(()=>Time.timeScale = 0);
          }
          
          yield return new WaitForSecondsRealtime(4f);
          PhotonNetwork.LeaveRoom();
          PhotonNetwork.Disconnect();
          SceneManager.LoadScene(1);
     }
     
     [PunRPC]
     private void RPC_TakeDamageEnnemi(int i)
     {
          OwnLife -= i;
     }
     
     [PunRPC]
     private void RPC_TakeDamageHimself(int i)
     {
          EnnemiLife -= i;
     }
     
     
     
}
