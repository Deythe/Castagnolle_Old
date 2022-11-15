using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Create Dice List", fileName = "DiceList")]
public class DiceListScriptable : ScriptableObject
{
    [Serializable]
    public class Dice
    {
        public string name;
        public Sprite sprite;
        public enumRessources[] faces;
    }
    
    public enum enumRessources
    {
        Nothing,Whatever,Red, Purple, Blue, Neutral, Milk, DoubleRed, DoublePurple, DoubleBlue, DoubleNeutral, TripleRed, TriplePurple, TripleBlue, TripleNeutral
    }

    public List<Dice> dicesList;
    public List<Texture2D> textureList;
    public List<Sprite> symbolsList;
}
