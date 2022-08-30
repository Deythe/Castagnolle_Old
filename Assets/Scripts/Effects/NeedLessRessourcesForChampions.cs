using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NeedLessRessourcesForChampions : MonoBehaviour, IEffects
{
    public static Dictionary<GameObject, DiceListScriptable.enumRessources[]> originalCard;
    public static List<GameObject> unitOnBoard;
    public static GameObject motherUnit;
    public static int degatMore=0;
    
    [SerializeField] private PhotonView view;
    [SerializeField] private bool used;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;

    private int numberUnitCurrent;
    private DiceListScriptable.enumRessources[] pivotRessourceList;
    private int pivot;
    
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (usingPhases[0].Equals(phase))
        {
            if (view.AmOwner)
            {
                numberUnitCurrent = PlacementManager.instance.GetBoard().Count;

                if (originalCard == null)
                {
                    originalCard = new Dictionary<GameObject, DiceListScriptable.enumRessources[]>();

                    foreach (var card in DeckManager.instance.CardDeck)
                    {
                        if (card.GetComponent<CardData>().IsChampion)
                        {
                            pivotRessourceList = new DiceListScriptable.enumRessources[card.GetComponent<CardData>().p_ressources.Count];
                            for (int i = 0; i < card.GetComponent<CardData>().p_ressources.Count; i++)
                            {
                                pivotRessourceList[i] = card.GetComponent<CardData>().p_ressources[i];
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
                    if (card.GetComponent<CardData>().IsChampion && card.GetComponent<CardData>().p_ressources.Count > 0)
                    {
                        card.GetComponent<CardData>().p_ressources
                            .RemoveAt(card.GetComponent<CardData>().p_ressources.Count - 1);
                    }
                }
                
                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
            }
        }else if (phase == EffectManager.enumEffectPhaseActivation.WhenThisUnitDie)
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
                                .Equals(card.GetComponent<CardData>().p_ressources.Count))
                            {
                                card.GetComponent<CardData>().p_ressources
                                    .Add(originalCard[card.GetComponent<CardData>().Prefabs][
                                        card.GetComponent<CardData>().p_ressources.Count]);
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
                    card.GetComponent<CardData>().p_ressources.Clear();
                    card.GetComponent<CardData>().p_ressources =
                        new List<DiceListScriptable.enumRessources>(originalCard[card.GetComponent<CardData>().Prefabs]);
                }
            }
        }
    }
    

    [PunRPC]
    private void RPC_Action(int id, int atk)
    { 
        PlacementManager.instance.SearchMobWithID(id).p_atk+=atk;
    }
    

    List<EffectManager.enumEffectPhaseActivation> IEffects.GetPhaseActivation()
    {
        return usingPhases;
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