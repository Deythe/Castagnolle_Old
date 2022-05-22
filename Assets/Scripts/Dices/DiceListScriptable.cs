using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Create Dice List", fileName = "DiceList")]
public class DiceListScriptable : ScriptableObject
{
    public List<DiceScriptable> diceDeck;
    public List<Texture2D> textureList;
    public List<Sprite> symbolsList;
}
