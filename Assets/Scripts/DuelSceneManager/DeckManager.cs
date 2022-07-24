using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;
    [SerializeField] private List<GameObject> cardDeck = new List<GameObject>();
    [SerializeField] private CardListScriptable cardListDeck;
    
    [SerializeField] private int[] checks = new int[6];
    
    private int[] ressources;
    private int check;
    public List<GameObject> CardDeck
    {
        get => cardDeck;
        set
        {
            cardDeck = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitDeck();
    }


    private void InitDeck()
    {
        for (int i = 0; i < FireBaseManager.instance.User.currentDeck.Length; i++)
        {
            cardDeck.Add(UiManager.instance.InitCard(cardListDeck.cards[FireBaseManager.instance.User.currentDeck[i]]));
        }
    }

    public void CheckUnitWithRessources()
    {
        for (int i = 0; i < cardDeck.Count; i++)
        {
            InitCheck();
            ressources = cardDeck[i].GetComponent<CardData>().Ressources.ToArray();
            
            for (int j = 0; j < ressources.Length; j++)
            {
                if (!ressources[j].Equals(4))
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
                    else if (ressources[j] == checks[3])
                    {
                        checks[3] = 0;
                        ressources[j] = 0;
                    }
                    else if (ressources[j] == checks[4])
                    {
                        checks[4] = 0;
                        ressources[j] = 0;
                    }
                    else if (ressources[j] == checks[5])
                    {
                        checks[5] = 0;
                        ressources[j] = 0;
                    }
                }
                else
                {
                    HaveAResource(j);
                }
            }

            if (AllCheckValide(ressources) && !(PlacementManager.instance.HaveAChampionOnBoard && cardDeck[i].GetComponent<CardData>().IsChampion))
            {
                cardDeck[i].GetComponent<CardData>().p_enabled = true;
            }
            else
            {
                cardDeck[i].GetComponent<CardData>().p_enabled = false;
                UiManager.instance.MoveCardAtEnd(cardDeck[i]);
            }
        }
    }

    private void InitCheck()
    {
        for (int j = 0; j < DiceManager.instance.DiceChoosen.Length+DiceManager.instance.Gauge.Length; j++)
        {
            if (j < DiceManager.instance.DiceChoosen.Length)
            {
                checks[j] = DiceManager.instance.DiceChoosen[j];
            }
            else
            {
                checks[j] = DiceManager.instance.Gauge[j-DiceManager.instance.Gauge.Length];
            }
        }
    }

    private void HaveAResource(int j)
    {
        for(int k = 0; k < checks.Length; k++)
        {
            if (checks[k] != 0)
            {
                ressources[j] = 0;
                checks[k] = 0;
                return;
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


}
