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
     
     public void TakeDamageHimself(int i)
     {
          life -= i;
          lifeManagerView.RPC("RPC_TakeDamageHimself", RpcTarget.Others, i);
     }

     public void TakeDamageEnnemi(int i)
     {
          ennemiLife -= i;
          lifeManagerView.RPC("RPC_TakeDamageEnnemi", RpcTarget.Others, i);
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
