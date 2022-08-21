using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffects
{
    public void OnCast(EffectManager.enumEffectPhaseActivation phase);
    
    public List<EffectManager.enumEffectPhaseActivation> GetPhaseActivation();
    public List<EffectManager.enumConditionEffect> GetConditionsForActivation();
    
    public bool GetUsed();

    public void SetUsed(bool b);
}
