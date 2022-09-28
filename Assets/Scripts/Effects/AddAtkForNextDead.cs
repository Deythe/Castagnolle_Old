using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AddAtkForNextDead : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (usingPhases[0]==phase)
        {
            switch (CastagneManager.instance.p_result)
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
        GetComponent<MonstreData>().p_atk+=number;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = effectMother.GetView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
        isEffectAuto = effectMother.GetIsEffectAuto();
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectPhaseActivation> GetUsingPhases()
    {
        return usingPhases;
    }
    
    public List<EffectManager.enumConditionEffect> GetConditions()
    {
        return conditions;
    }
    
    public bool GetIsActivable()
    {
        return isActivable;
    }

    public void SetIsActivable(bool b)
    {
        isActivable = b;
    }

    public bool GetUsed()
    {
        return used;
    }

    public void SetUsed(bool b)
    {
        used = b;
    }

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
}
