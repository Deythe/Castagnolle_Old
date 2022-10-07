using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class AddAtkForNextDead : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;


    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (conditions[0]==condition)
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
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectConditionActivation> GetConditions()
    {
        return conditions;
    }
    
    public List<EffectManager.enumActionEffect> GetActions()
    {
        return actions;
    }
    
    public bool GetIsActivable()
    {
        return isActivable;
    }

    public void SetIsActivable(bool b)
    {
        isActivable = b;
    }

    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
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
