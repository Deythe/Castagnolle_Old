using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create New Dice", fileName = "Dice")]
public class DiceScriptable : ScriptableObject
{
    public int[] faces = new int[6];
}
