using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NeedLessRessources : MonoBehaviour, IEffects
{
    public static Dictionary<GameObject, int[]> originalCard;

    [SerializeField] private PhotonView view;
    
    private int numberUnitCurrent;
    private int usingPhase = 0;
    public static bool used = false;
    private int[] pivotRessourceList;
    private int pivot;
    
    public void OnCast(int phase)
    {
        if (usingPhase == phase)
        {
            if (view.AmOwner)
            {
                numberUnitCurrent = PlacementManager.instance.GetBoard().Count;

                if (originalCard == null)
                {
                    originalCard = new Dictionary<GameObject, int[]>();

                    foreach (var card in DeckManager.instance.CardDeck)
                    {
                        if (card.GetComponent<CardData>().IsChampion)
                        {
                            pivotRessourceList = new int[card.GetComponent<CardData>().Ressources.Count];
                            for (int i = 0; i < card.GetComponent<CardData>().Ressources.Count; i++)
                            {
                                pivotRessourceList[i] = card.GetComponent<CardData>().Ressources[i];
                            }

                            originalCard.Add(card.GetComponent<CardData>().Prefabs, pivotRessourceList);
                        }
                    }
                }

                foreach (var card in DeckManager.instance.CardDeck)
                {
                    if (card.GetComponent<CardData>().IsChampion && card.GetComponent<CardData>().Ressources.Count > 0)
                    {
                        card.GetComponent<CardData>().Ressources
                            .RemoveAt(card.GetComponent<CardData>().Ressources.Count - 1);
                    }
                }
                
                EffectManager.instance.CancelSelection();
            }
        }
    }

    private void Update()
    {
        if (view.AmOwner && !used)
        {
            if (numberUnitCurrent < PlacementManager.instance.GetBoard().Count)
            {
                numberUnitCurrent = PlacementManager.instance.GetBoard().Count;
                if (PlacementManager.instance.GetBoard()[numberUnitCurrent - 1].monster.GetComponent<Monster>()
                    .IsChampion)
                {
                    PlacementManager.instance.GetBoard()[numberUnitCurrent - 1].monster.GetComponent<Monster>().Atk++;
                    used = true;
                    foreach (var card in DeckManager.instance.CardDeck)
                    {
                        if (card.GetComponent<CardData>().IsChampion)
                        {
                            card.GetComponent<CardData>().Ressources.Clear();
                            
                            for (int i = 0; i < card.GetComponent<CardData>().Ressources.Capacity; i++)
                            {
                                pivot = originalCard[card.GetComponent<CardData>().Prefabs][i];
                                card.GetComponent<CardData>().Ressources.Add(pivot);
                            }
                            
                            DeckManager.instance.CheckUnitWithRessources();
                            UiManager.instance.UpdateListCard();
                        }
                    }
                }
            }
        }
    }

    public int GetPhaseActivation()
    {
        return usingPhase;
    }

    public bool GetUsed()
    {
        return used;
    }
    
    public void SetUsed(bool b)
    {
        used = b;
    }
}
