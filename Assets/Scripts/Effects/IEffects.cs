using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public interface IEffects
{
    public void OnCast(EffectManager.enumEffectPhaseActivation phase);

    public PhotonView GetView();
    public List<EffectManager.enumEffectPhaseActivation> GetUsingPhases();
    public List<EffectManager.enumConditionEffect> GetConditions();
    
    public bool GetIsActivable();
    public void SetIsActivable(bool b);
    
    public bool GetUsed();
    public void SetUsed(bool b);
    public void TransferEffect(IEffects effectMother);
}
