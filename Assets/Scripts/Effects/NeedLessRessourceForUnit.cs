using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NeedLessRessourceForUnit : MonoBehaviour, IEffects
{
    public GameObject originalCard;
    
    public static List<GameObject> unitOnBoard;
    public static GameObject motherUnit;
    
    [SerializeField] private PhotonView view;
    [SerializeField] private bool used;
    
    private int numberUnitCurrent;
    private int usingPhase = 0;
    
    private int[] pivotRessourceList;
    private int pivot;
    
    public void OnCast(int phase)
    {
        if (usingPhase == phase)
        {
            if (!view.AmOwner)
            {
                Debug.Log("Poupouleeee");
                numberUnitCurrent = PlacementManager.instance.GetBoard().Count;

                if (unitOnBoard == null)
                {
                    unitOnBoard = new List<GameObject>();
                }
                
                unitOnBoard.Add(gameObject);
                
                if (motherUnit == null)
                {
                    motherUnit = gameObject;
                }

                foreach (var card in DeckManager.instance.CardDeck)
                {
                    if (card.GetComponent<CardData>().Prefabs.Equals(originalCard.GetComponent<CardData>().Prefabs))
                    {
                        card.GetComponent<CardData>().Ressources
                            .RemoveAt(card.GetComponent<CardData>().Ressources.Count - 1);
                    }
                }
                
                EffectManager.instance.CancelSelection(1);
            }
        }
    }

    private void Update()
    {
        if (!view.AmOwner)
        {
            if (numberUnitCurrent > PlacementManager.instance.GetBoard().Count)
            {
                numberUnitCurrent = PlacementManager.instance.GetBoard().Count;
            } else if (numberUnitCurrent < PlacementManager.instance.GetBoard().Count)
            {
                numberUnitCurrent = PlacementManager.instance.GetBoard().Count;
                
                if (gameObject.Equals(motherUnit))
                {
                    if (PlacementManager.instance.GetBoard()[numberUnitCurrent - 1].monster.GetComponent<PhotonView>()
                        .AmOwner)
                    {
                        if (PlacementManager.instance.GetBoard()[numberUnitCurrent - 1].monster.Equals(originalCard.GetComponent<CardData>().Prefabs))
                        {
                            DeckManager.instance.CheckUnitWithRessources();
                            UiManager.instance.UpdateListCard();
                        }
                    }
                }
            }
        }
    }

    void ResetUnit()
    {
        foreach (var card in DeckManager.instance.CardDeck)
        {
            if (card.GetComponent<CardData>().Prefabs.Equals(originalCard.GetComponent<CardData>().Prefabs))
            {
                card.GetComponent<CardData>().Ressources.Clear();
                card.GetComponent<CardData>().Ressources = originalCard.GetComponent<CardData>().Ressources;
            }
        }
    }

    private void OnDestroy()
    {
        if (view.AmOwner)
        {
            if (gameObject.Equals(motherUnit))
            {
                //Debug.Log("WasMotherUnit");
                if (unitOnBoard.Count > 1)
                {
                    //Debug.Log("New Mother UNit");
                    motherUnit = unitOnBoard[1];
                }
                else
                {
                    motherUnit = null;
                    ResetUnit();
                    return;
                }
            }
            
            //Debug.Log("Destroy");
            foreach (var card in DeckManager.instance.CardDeck)
            {
                if (card != null)
                {
                    if (card.GetComponent<CardData>().Prefabs.Equals(originalCard.GetComponent<CardData>().Prefabs))
                    {
                        if (!card.GetComponent<CardData>().Ressources.Count.Equals(originalCard.GetComponent<CardData>().Ressources.Count))
                        {
                            card.GetComponent<CardData>().Ressources.Add(originalCard.GetComponent<CardData>().Ressources[ card.GetComponent<CardData>().Ressources.Count]);
                        }
                    }
                }
            }

            unitOnBoard.Remove(gameObject);
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
