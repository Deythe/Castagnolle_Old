using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffects
{
    public void OnCast(int phase);

    public int GetPhaseActivation(); // 0 = quand c placé, 1 = quand tu tue, 2 = quand tu meurt, 3 = Activable, 4 = Activable2, 5 = quand une créature meurt;

    public bool GetUsed();

    public void SetUsed(bool b);
}
