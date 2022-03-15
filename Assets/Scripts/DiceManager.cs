using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;
    
    [SerializeField] private List<DiceScriptable> diceDeck;
    
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
        
        DeckManager.instance.CheckUnitWithRessources();
        RoundManager.instance.SetStateRound(1);
        UiManager.instance.UpdateListCard();
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

    public void DeleteAllResources(List<int> resource)
    {
        for (int i = 0; i < resource.Count; i++)
        {
            DeleteResource(resource[i]);
        }
        
        DeckManager.instance.CheckUnitWithRessources();
        UiManager.instance.UpdateListCard();
    }

    public void DeleteResource(int i)
    {
        for (int j = 0; j < diceChoosen.Length; j++)
        {
            if (i.Equals(diceChoosen[j]))
            {
                diceChoosen[j] = 0;
                return;
            }
        }
    }

    private int PickDice()
    {
        return diceDeck[Random.Range(0, diceDeck.Count)].faces[Random.Range(0, 6)];
    }

    public int[] GetDiceChoosen()
    {
        return diceChoosen;
    }
}
