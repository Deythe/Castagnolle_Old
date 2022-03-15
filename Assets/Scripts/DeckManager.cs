using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    [SerializeField] private List<GameObject> cardDeck;
    
    private List<GameObject> monsterPossible = new List<GameObject>();
    private int[] checks = new int[3];
    private int[] ressources;
    private void Awake()
    {
        instance = this;
    }

    public void CheckUnitWithRessources()
    {
        monsterPossible.Clear();
        
        for (int i = 0; i < cardDeck.Count; i++)
        {
            Array.Copy(DiceManager.instance.GetDiceChoosen(), checks, 3);
            ressources = cardDeck[i].GetComponent<CardData>().GetStat().resources.ToArray();
 
            for (int j = 0; j < ressources.Length; j++)
            {
                if (ressources[j] == checks[0])
                {
                    checks[0] = 0;
                    ressources[j] = 0;
                }
                else if (ressources[j] == checks[1])
                {
                    checks[1] = 0;
                    ressources[j] = 0;
                }
                else if (ressources[j] == checks[2])
                {
                    checks[2] = 0;
                    ressources[j] = 0;
                }
            }

            if (AllCheckValide(ressources))
            {
                monsterPossible.Add(cardDeck[i]);
            }
        }
    }

    public void DeleteCartFromDeck(GameObject go)
    {
        for (int i = 0; i < cardDeck.Count; i++)
        {
            if (go.Equals(cardDeck[i]))
            {
                cardDeck.RemoveAt(i);
            }
        }
    }

    bool AllCheckValide(int[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i]!=0)
            {
                return false;
            }
        }

        return true;
    }

    public List<GameObject> GetMonsters()
    {
        return monsterPossible;
    }


}
