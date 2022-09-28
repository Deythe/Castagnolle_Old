using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NeedLessRessourcesForChampions : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    public static Dictionary<GameObject, DiceListScriptable.enumRessources[]> originalCard;
    public static List<GameObject> unitOnBoard;
    public static GameObject motherUnit;
    public static int degatMore=0;
    
    
    private int numberUnitCurrent;
    private DiceListScriptable.enumRessources[] pivotRessourceList;
    private int pivot;
    private void Awake()
    {
        isActivable = true;
    }
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases.Contains(phase))
            {
                numberUnitCurrent = PlacementManager.instance.p_board.Count;

                if (originalCard == null)
                {
                    originalCard = new Dictionary<GameObject, DiceListScriptable.enumRessources[]>();

                    foreach (var card in DeckManager.instance.CardDeck)
                    {
                        if (card.GetComponent<CardData>().p_isChampion)
                        {
                            pivotRessourceList = new DiceListScriptable.enumRessources[card.GetComponent<CardData>().p_ressources.Count];
                            for (int i = 0; i < card.GetComponent<CardData>().p_ressources.Count; i++)
                            {
                                pivotRessourceList[i] = card.GetComponent<CardData>().p_ressources[i];
                            }

                            originalCard.Add(card.GetComponent<CardData>().p_prefabs, pivotRessourceList);
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
                    if (card.GetComponent<CardData>().p_isChampion && card.GetComponent<CardData>().p_ressources.Count > 0)
                    {
                        card.GetComponent<CardData>().p_ressources
                            .RemoveAt(card.GetComponent<CardData>().p_ressources.Count - 1);
                    }
                }
                
                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
            }
        }
        
        if (phase == EffectManager.enumEffectPhaseActivation.WhenThisUnitDie)
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
                        if (originalCard.ContainsKey(card.GetComponent<CardData>().p_prefabs))
                        {
                            if (!originalCard[card.GetComponent<CardData>().p_prefabs].Length
                                .Equals(card.GetComponent<CardData>().p_ressources.Count))
                            {
                                card.GetComponent<CardData>().p_ressources
                                    .Add(originalCard[card.GetComponent<CardData>().p_prefabs][
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
            if (numberUnitCurrent > PlacementManager.instance.p_board.Count)
            {
                numberUnitCurrent = PlacementManager.instance.p_board.Count;
            } else if (numberUnitCurrent < PlacementManager.instance.p_board.Count)
            {
                numberUnitCurrent = PlacementManager.instance.p_board.Count;
                
                if (gameObject.Equals(motherUnit))
                {
                    if (PlacementManager.instance.p_board[numberUnitCurrent - 1].monster.GetComponent<PhotonView>()
                        .AmOwner)
                    {
                        if (PlacementManager.instance.p_board[numberUnitCurrent - 1].monster.GetComponent<MonstreData>()
                            .p_isChampion)
                        {
                            view.RPC("RPC_Action", RpcTarget.AllViaServer,
                                PlacementManager.instance.p_board[numberUnitCurrent - 1].monster
                                    .GetComponent<PhotonView>().ViewID, degatMore);
                            
                            degatMore = 0;
                            ResetUnit();
                            DeckManager.instance.CheckUnitWithRessources();
                            motherUnit = null;
                            unitOnBoard.Clear();
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
                if (card.GetComponent<CardData>().p_isChampion)
                {
                    card.GetComponent<CardData>().p_ressources.Clear();
                    card.GetComponent<CardData>().p_ressources =
                        new List<DiceListScriptable.enumRessources>(originalCard[card.GetComponent<CardData>().p_prefabs]);
                }
            }
        }
    }
    

    [PunRPC]
    private void RPC_Action(int id, int atk)
    { 
        PlacementManager.instance.SearchMobWithID(id).p_atk+=atk;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = effectMother.GetView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
        isEffectAuto = effectMother.GetIsEffectAuto();
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectPhaseActivation> GetUsingPhases()
    {
        return usingPhases;
    }
    
    public List<EffectManager.enumConditionEffect> GetConditions()
    {
        return conditions;
    }
    
    public bool GetIsActivable()
    {
        return isActivable;
    }

    public void SetIsActivable(bool b)
    {
        isActivable = b;
    }

    public bool GetUsed()
    {
        return used;
    }

    public void SetUsed(bool b)
    {
        used = b;
    }

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
}
