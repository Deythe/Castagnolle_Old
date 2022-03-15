using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    [SerializeField] private MonsterCardScriptable stats;
    
    public MonsterCardScriptable GetStat()
    {
        return stats;
    }

}
