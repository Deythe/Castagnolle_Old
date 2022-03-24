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

     private void Awake()
     {
          instance = this;
     }

     private void Start()
     {
          life = 20;
          ennemiLife = 20;
     }

     public void TakeDamageHimself(int degats)
     {
          life -= 1;
          lifeManagerView.RPC("RPC_TakeDamageHimself", RpcTarget.Others, 1);
     }


     public void TakeDamageEnnemi(float i)
     {
          int result=0;
          
          if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"] == 1)
          {
               if (i < -0.5)
               {
                    Debug.Log("test1");
                    result = 1;
               }
               else if (i.Equals(0.5f) || i.Equals(1.5f))
               {
                    Debug.Log("test2");
                    result = 2;
               }
               else if(i.Equals(2.5f) || i.Equals(3.5f))
               {
                    Debug.Log("test3");
                    result = 4;
               }else if (i.Equals(4.5f))
               {
                    Debug.Log("test4");
                    result = 6;
               }
          }
          else
          {
               if (i > 0.5)
               {
                    Debug.Log("test1");
                    result = 1;
               }
               else if (i.Equals(-0.5f) || i.Equals(-1.5f))
               {
                    Debug.Log("test2");
                    result = 2;
               }
               else if(i.Equals(-2.5f) || i.Equals(-3.5f))
               {
                    Debug.Log("test3");
                    result = 4;
               }else if (i.Equals(-4.5f))
               {
                    Debug.Log("test4");
                    result = 6;
               }
          }
          Debug.Log(result);
          
          ennemiLife -= result;
          lifeManagerView.RPC("RPC_TakeDamageEnnemi", RpcTarget.Others, result);
     }

     public int GetOwnLife()
     {
          return life;
     }
     
     public int GetEnnemiLife()
     {
          return life;
     }

     public void CheckEndGame()
     {
          if (ennemiLife <= 0 ||life <= 0 )
          {
               PhotonNetwork.Disconnect();
          }
     }
     
     [PunRPC]
     private void RPC_TakeDamageEnnemi(int i)
     {
          life -= i;
     }
     
     [PunRPC]
     private void RPC_TakeDamageHimself(int i)
     {
          ennemiLife -= i;
     }
     
     
     
}
