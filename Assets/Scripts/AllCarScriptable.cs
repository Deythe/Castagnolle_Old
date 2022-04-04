using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create List Of Cards", fileName = "CardList")]
public class AllCarScriptable : ScriptableObject
{
    public List<GameObject> cards;
}
