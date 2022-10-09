using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public interface IEffects
{
    public void OnCast(EffectManager.enumEffectConditionActivation condition);

    public PhotonView GetView();
    public List<EffectManager.enumEffectConditionActivation> GetConditions();
    public List<EffectManager.enumActionEffect> GetActions();
    
    public bool GetIsActivable();
    public void SetIsActivable(bool b);

    public EffectManager.enumOrderPriority GetOrderPriority();
    public bool GetUsed();
    public void SetUsed(bool b);
    public void TransferEffect(IEffects effectMother);
    public void ResetEffect();
}
