using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create THE LIST OF UNIT IN THE GAME", fileName = "UNITLIST")]
public class UnitListScriptable : ScriptableObject
{
    public List<Card> cards;
}

[Serializable]
public class Card
{
    public GameObject miniaCard;
    public GameObject model;
    public Sprite cardStats;
}
