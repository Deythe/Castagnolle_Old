using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NeedLessRessourcesForChampions : MonoBehaviour, IEffects
{
    public static Dictionary<GameObject, int[]> originalCard;
    public static List<GameObject> unitOnBoard;
    public static GameObject motherUnit;
    public static int degatMore=0;
    
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

                if (unitOnBoard == null)
                {
                    unitOnBoard = new List<GameObject>();
                }
                
                unitOnBoard.Add(gameObject);
                
                if (motherUnit == null)
                {
                    motherUnit = gameObject;
                }
                
                degatMore++;

                foreach (var card in DeckManager.instance.CardDeck)
                {
                    if (card.GetComponent<CardData>().IsChampion && card.GetComponent<CardData>().Ressources.Count > 0)
                    {
                        card.GetComponent<CardData>().Ressources
                            .RemoveAt(card.GetComponent<CardData>().Ressources.Count - 1);
                    }
                }
                
                DeckManager.instance.CheckUnitWithRessources();
                UiManager.instance.UpdateListCard();
                EffectManager.instance.CancelSelection(1);
            }
        }else if (phase == 2)
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
                        degatMore--;
                    }
                    else
                    {
                        degatMore = 0;
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
                        if (originalCard.ContainsKey(card.GetComponent<CardData>().Prefabs))
                        {
                            if (!originalCard[card.GetComponent<CardData>().Prefabs].Length
                                .Equals(card.GetComponent<CardData>().Ressources.Count))
                            {
                                card.GetComponent<CardData>().Ressources
                                    .Add(originalCard[card.GetComponent<CardData>().Prefabs][
                                        card.GetComponent<CardData>().Ressources.Count]);
                            }
                        }
                    }
                }

                unitOnBoard.Remove(gameObject);
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
                        if (PlacementManager.instance.GetBoard()[numberUnitCurrent - 1].monster.GetComponent<Monster>()
                            .p_isChampion)
                        {
                            view.RPC("RPC_Action", RpcTarget.AllViaServer,
                                PlacementManager.instance.GetBoard()[numberUnitCurrent - 1].monster
                                    .GetComponent<PhotonView>().ViewID, degatMore);
                            
                            //degatMore = 0;
                            //ResetUnit();
                            DeckManager.instance.CheckUnitWithRessources();
                            UiManager.instance.UpdateListCard();
                            //motherUnit = null;
                            //unitOnBoard.Clear();
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
            if (card != null)
            {
                if (card.GetComponent<CardData>().IsChampion)
                {
                    card.GetComponent<CardData>().Ressources.Clear();
                    card.GetComponent<CardData>().Ressources =
                        new List<int>(originalCard[card.GetComponent<CardData>().Prefabs]);
                }
            }
        }
    }

    private void OnDestroy()
    {/*
        if (view.AmOwner)
        {
            if (gameObject.Equals(motherUnit))
            {
                //Debug.Log("WasMotherUnit");
                if (unitOnBoard.Count > 1)
                {
                    //Debug.Log("New Mother UNit");
                    motherUnit = unitOnBoard[1];
                    degatMore--;
                }
                else
                {
                    degatMore = 0;
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
                    if (originalCard.ContainsKey(card.GetComponent<CardData>().Prefabs))
                    {
                        if (!originalCard[card.GetComponent<CardData>().Prefabs].Length
                            .Equals(card.GetComponent<CardData>().Ressources.Count))
                        {
                            card.GetComponent<CardData>().Ressources
                                .Add(originalCard[card.GetComponent<CardData>().Prefabs][
                                    card.GetComponent<CardData>().Ressources.Count]);
                        }
                    }
                }
            }

            unitOnBoard.Remove(gameObject);
        }*/
    }

    [PunRPC]
    private void RPC_Action(int id, int atk)
    { 
        PlacementManager.instance.SearchMobWithID(id).Atk+=atk;
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