using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;
    
    [SerializeField] private List<DiceScriptable> diceDeck;
    
    [SerializeField] private int[] diceChoosen = new int[3];
    [SerializeField] private int[] diceGauge = new int[3];
    
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
        for (int i = 0; i < 3; i++)
        {
            if (diceChoosen[i]!=0 || diceGauge[i]!=0)
            {
                return false;
            }
        }
        
        return true;
    }

    public void PutInGauge(int i)
    {
        if (diceChoosen[i] != 0)
        {
            diceGauge[i] = diceChoosen[i];
            diceChoosen[i] = 0;
            DeckManager.instance.CheckUnitWithRessources();
            UiManager.instance.UpdateListCard();
        }
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
        for (int j = 0; j < diceChoosen.Length+diceGauge.Length; j++)
        {
            if (j < diceChoosen.Length)
            {
                if (i.Equals(diceChoosen[j]))
                {
                    diceChoosen[j] = 0;
                    return;
                }
            }
            else
            {
                if (i.Equals(diceGauge[j-diceGauge.Length]))
                {
                    diceGauge[j-diceGauge.Length] = 0;
                    return;
                }
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
    
    public int[] GetGauge()
    {
        return diceGauge;
    }
}
