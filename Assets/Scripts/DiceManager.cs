using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;
    
    private List<int> diceDeck;
    private int[] diceChoosen = new int[3];

    private void Awake()
    {
        instance = this;
    }

    public void ChooseDice()
    {
        for (int i = 0; i < diceChoosen.Length; i++)
        {
            diceChoosen[i] = PickDice();
        }
        RoundManager.instance.SetStateRound(1);
    }

    public bool DeckEmpy()
    {
        foreach (var dice in diceChoosen)
        {
            if (dice!=0)
            {
                return false;
            }
        }

        return true;
    }

    private int PickDice()
    {
        return Random.Range(1, 7);
    }

    public int[] GetDiceDeck()
    {
        return diceChoosen;
    }
}
