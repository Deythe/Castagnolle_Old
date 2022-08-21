using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AddAtkForNextDead : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    private bool used;
    
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (usingPhases[0]==phase)
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

    public List<EffectManager.enumEffectPhaseActivation> GetPhaseActivation()
    {
        return usingPhases;
    }

    public List<EffectManager.enumConditionEffect> GetConditionsForActivation()
    {
        return conditions;
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
