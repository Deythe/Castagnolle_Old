using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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
               ennemiLife = value;
               UiManager.instance.UpdateHpEnnemi();
               CheckWinGame();
          }
     }
     
     public int OwnLife
     {
          get => life;
          set
          {
               life = value;
               UiManager.instance.UpdateHp();
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
               if (i <= -0.5)
               {
                    result = 1;
               }
               else if (i.Equals(0.5f) || i.Equals(1.5f))
               {
                    result = 2;
               }
               else if(i.Equals(2.5f) || i.Equals(3.5f))
               {
                    result = 4;
               }else if (i.Equals(4.5f))
               {
                    result = 6;
               }
          }
          else
          {
               if (i >= 0.5)
               {
                    result = 1;
               }
               else if (i.Equals(-0.5f) || i.Equals(-1.5f))
               {
                    result = 2;
               }
               else if(i.Equals(-2.5f) || i.Equals(-3.5f))
               {
                    result = 4;
               }else if (i.Equals(-4.5f))
               {
                    result = 6;
               }
          }
          
          EnnemiLife -= result;
          lifeManagerView.RPC("RPC_TakeDamageEnnemi", RpcTarget.Others, result);
     }

     public void CheckWinGame()
     {
          if (ennemiLife <= 0)
          {
               PhotonNetwork.LeaveRoom();
               PhotonNetwork.Disconnect();
          }
     }
     
     public void CheckLoseGame()
     {
          if (life <= 0)
          {
               PhotonNetwork.LeaveRoom();
               PhotonNetwork.Disconnect();
          }
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
