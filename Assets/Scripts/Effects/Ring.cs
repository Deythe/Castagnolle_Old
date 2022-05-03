using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Ring : MonoBehaviour,IEffects
{
    private int numberMonster;
    private int usingPhase = 1;
    private bool used;
    private int kill = 0;

    private void Start()
    {
        numberMonster = PhotonNetwork.ViewCount;
    }

    private void Update()
    {
        if (numberMonster < PhotonNetwork.ViewCount)
        {
            OnCast(0);
        }
    }

    public void OnCast(int phase)
    {
        if ( BattlePhaseManager.instance.UnitSelected!=null)
        {
            BattlePhaseManager.instance.UnitSelected.GetComponent<Monster>().Atk++;
        }
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
