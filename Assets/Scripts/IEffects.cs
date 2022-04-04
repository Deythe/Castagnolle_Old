using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffects
{
    public void OnCast();

    public int GetPhaseActivation(); // 0 = quand c placé, 1 = quand tu tue, 2 = quand tu meurt

    public bool GetUsed();

    public void SetUsed(bool b);
}
