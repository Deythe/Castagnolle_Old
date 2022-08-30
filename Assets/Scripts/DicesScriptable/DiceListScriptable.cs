using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Create Dice List", fileName = "DiceList")]
public class DiceListScriptable : ScriptableObject
{
    public enum enumRessources
    {
        Whatever,Red, Purple, Blue, Neutral, DoubleRed, DoublePurple, DoubleBlue, DoubleNeutral, TripleRed, TriplePurple, TripleBlue, TripleNeutral
    }

    public List<DiceScriptable> diceDeck;
    public List<enumRessources> textureIndex;
    public List<Texture2D> textureList;
    public List<Sprite> symbolsList;
}
