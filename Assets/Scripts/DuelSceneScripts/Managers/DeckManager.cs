using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;
    [SerializeField] private List<GameObject> cardDeck = new List<GameObject>();
    [SerializeField] private DiceListScriptable.enumRessources[] checks = new DiceListScriptable.enumRessources[6];
    private DiceListScriptable.enumRessources[] resources;
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
        for (int i = 0; i < FireBaseManager.instance.User.currentCardsDeck.Length; i++)
        {
            cardDeck.Add(UiManager.instance.InitCard(LocalSaveManager.instance.unitList.cards[LocalSaveManager.instance.user.currentCardsDeck[i]].miniaCard));
        }
    }

    public void CheckUnitWithRessources()
    {
        for (int i = 0; i < cardDeck.Count; i++)
        {
            InitCheck();
            resources = cardDeck[i].GetComponent<CardData>().p_ressources.ToArray();
            
            for (int j = 0; j < resources.Length; j++)
            {
                if (!resources[j].Equals(4))
                {
                    if (resources[j].Equals(checks[0]))
                    {
                        checks[0] = 0;
                        resources[j] = 0;
                    }
                    else if (resources[j].Equals(checks[1]))
                    {
                        checks[1] = 0;
                        resources[j] = 0;
                    }
                    else if (resources[j].Equals(checks[2]))
                    {
                        checks[2] = 0;
                        resources[j] = 0;
                    }
                    else if (resources[j].Equals(checks[3]))
                    {
                        checks[3] = 0;
                        resources[j] = 0;
                    }
                    else if (resources[j].Equals(checks[4]))
                    {
                        checks[4] = 0;
                        resources[j] = 0;
                    }
                    else if (resources[j].Equals(checks[5]))
                    {
                        checks[5] = 0;
                        resources[j] = 0;
                    }
                }
                else
                {
                    HaveAResource(j);
                }
            }

            if (AllCheckValide(resources) && !(PlacementManager.instance.p_haveAChampionOnBoard && cardDeck[i].GetComponent<CardData>().p_isChampion))
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
        for (int j = 0; j < DiceManager.instance.p_diceChoosen.Length+DiceManager.instance.p_diceGauge.Length; j++)
        {
            if (j < DiceManager.instance.p_diceChoosen.Length)
            {
                checks[j] = DiceManager.instance.p_diceChoosen[j];
            }
            else
            {
                checks[j] = DiceManager.instance.p_diceGauge[j-DiceManager.instance.p_diceGauge.Length];
            }
        }
    }

    private void HaveAResource(int j)
    {
        for(int k = 0; k < checks.Length; k++)
        {
            if (checks[k] != 0)
            {
                resources[j] = 0;
                checks[k] = 0;
                return;
            }
        }
    }
    
    bool AllCheckValide(DiceListScriptable.enumRessources[] list)
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
