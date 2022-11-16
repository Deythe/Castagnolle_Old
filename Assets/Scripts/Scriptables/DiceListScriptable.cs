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
        public List<enumRessources> faces;
    }

    public enum enumRessources
    {
        Nothing,Whatever,Red, Purple, Blue, Neutral, Milk, DoubleRed, DoublePurple, DoubleBlue, DoubleNeutral, TripleRed, TriplePurple, TripleBlue, TripleNeutral
    }

    public List<Sprite> symbolsList;
    public List<Dice> dicesList;
    public List<Texture2D> textureList;
    
    
    public Sprite ChooseGoodSprite(List<enumRessources> resources,int index)
    {
        switch (resources[index])
        {
            case enumRessources.Whatever:
                return symbolsList[0];
            case enumRessources.Red:
                return symbolsList[1];
            case enumRessources.Purple:
                return symbolsList[2];
            case enumRessources.Blue:
                return symbolsList[3];
            case enumRessources.Milk:
                return symbolsList[4];
        }

        return null;
    }
    
}
