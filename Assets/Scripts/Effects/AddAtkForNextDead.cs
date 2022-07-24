using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AddAtkForNextDead : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 5;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (usingPhase==phase)
        {
            switch (BattlePhaseManager.instance.Result)
            {
                case 0:
                    view.RPC("RPC_Action", RpcTarget.AllViaServer,
                        2);
                    break;

                case <0:
                case >0:
                    view.RPC("RPC_Action", RpcTarget.AllViaServer,
                        1);
                    break;
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action(int number)
    {
        GetComponent<Monster>().p_atk+=number;
    }


    public int GetPhaseActivation()
    {
        return usingPhase;
    }

    public bool GetUsed()
    {
        return used;
    }
    
    public void SetUsed(bool b)
    {
        used = b;
    }
}
