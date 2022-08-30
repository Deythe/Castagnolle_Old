using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NeedLessRessourceForUnit : MonoBehaviour, IEffects
{
    public GameObject originalCard;
    
    public static List<GameObject> unitOnBoard;
    public static GameObject motherUnit;
    
    [SerializeField] private PhotonView view;
    [SerializeField] private bool used;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhase;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;

    private int numberUnitCurrent;
    private int[] pivotRessourceList;
    private int pivot;
    
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (usingPhase[0].Equals(phase))
        {
            if (view.AmOwner)
            {
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
                        if (card.GetComponent<CardData>().p_ressources.Count > 0)
                        {
                            card.GetComponent<CardData>().p_ressources
                                .RemoveAt(card.GetComponent<CardData>().p_ressources.Count - 1);
                        }
                    }
                }
                
                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
            }
        }
    }

    private void Update()
    {
        if (view.AmOwner)
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
                card.GetComponent<CardData>().p_ressources.Clear();
                card.GetComponent<CardData>().p_ressources = originalCard.GetComponent<CardData>().p_ressources;
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
                        if (!card.GetComponent<CardData>().p_ressources.Count.Equals(originalCard.GetComponent<CardData>().p_ressources.Count))
                        {
                            card.GetComponent<CardData>().p_ressources.Add(originalCard.GetComponent<CardData>().p_ressources[ card.GetComponent<CardData>().p_ressources.Count]);
                        }
                    }
                }
            }

            unitOnBoard.Remove(gameObject);
        }
    }

    List<EffectManager.enumEffectPhaseActivation> IEffects.GetPhaseActivation()
    {
        return usingPhase;
    }

    public List<EffectManager.enumConditionEffect> GetConditionsForActivation()
    {
        return conditions;
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
