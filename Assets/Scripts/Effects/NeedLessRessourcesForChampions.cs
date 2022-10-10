using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NeedLessRessourcesForChampions : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;


    public static Dictionary<GameObject, DiceListScriptable.enumRessources[]> originalCardResources;
    public static Dictionary<GameObject, int> originalCardAtk;
    
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
    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                numberUnitCurrent = PlacementManager.instance.p_board.Count;

                if (originalCardResources == null)
                {
                    originalCardResources = new Dictionary<GameObject, DiceListScriptable.enumRessources[]>();
                    originalCardAtk = new Dictionary<GameObject, int>();
                    
                    foreach (var card in DeckManager.instance.CardDeck)
                    {
                        if (card.GetComponent<CardData>().p_isChampion)
                        {
                            pivotRessourceList = new DiceListScriptable.enumRessources[card.GetComponent<CardData>().p_ressources.Count];
                            for (int i = 0; i < card.GetComponent<CardData>().p_ressources.Count; i++)
                            {
                                pivotRessourceList[i] = card.GetComponent<CardData>().p_ressources[i];
                            }

                            originalCardResources.Add(card.GetComponent<CardData>().p_prefabs, pivotRessourceList);
                            originalCardAtk.Add(card.GetComponent<CardData>().p_prefabs, card.GetComponent<CardData>().p_atk);
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
                        card.GetComponent<CardData>().p_atk++;
                    }
                }
                
                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CancelSelection();
            }
            
            else if (condition == EffectManager.enumEffectConditionActivation.WhenThisUnitDie)
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
                            if (originalCardResources.ContainsKey(card.GetComponent<CardData>().p_prefabs))
                            {
                                if (!originalCardResources[card.GetComponent<CardData>().p_prefabs].Length
                                    .Equals(card.GetComponent<CardData>().p_ressources.Count))
                                {
                                    card.GetComponent<CardData>().p_ressources
                                        .Add(originalCardResources[card.GetComponent<CardData>().p_prefabs][
                                            card.GetComponent<CardData>().p_ressources.Count]);
                                    card.GetComponent<CardData>().p_atk--;
                                }
                            }
                        }
                    }

                    unitOnBoard.Remove(gameObject);
                }
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
            }
            else if (numberUnitCurrent < PlacementManager.instance.p_board.Count)
            {
                numberUnitCurrent = PlacementManager.instance.p_board.Count;

                if (PlacementManager.instance.p_board[numberUnitCurrent - 1].monster.GetComponent<PhotonView>()
                    .AmOwner)
                {
                    if (PlacementManager.instance.p_board[numberUnitCurrent - 1].monster.GetComponent<MonstreData>()
                        .p_isChampion)
                    {
                        used = true;
                        GetComponent<MonstreData>().p_model.layer = 6;
                        
                        if (gameObject.Equals(motherUnit))
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
                        new List<DiceListScriptable.enumRessources>(originalCardResources[card.GetComponent<CardData>().p_prefabs]);
                    card.GetComponent<CardData>().p_atk = originalCardAtk[card.GetComponent<CardData>().p_prefabs];
                }
            }
        }
    }
    

    [PunRPC]
    private void RPC_Action(int id, int atk)
    { 
        PlacementManager.instance.FindMobWithID(id).p_atk+=atk;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = gameObject.GetPhotonView();
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectConditionActivation> GetConditions()
    {
        return conditions;
    }
    
    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
    }
    
    public List<EffectManager.enumActionEffect> GetActions()
    {
        return actions;
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
    
    public void ResetEffect()
    {
        used = false;
    }

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
    
    public void CancelEffect()
    {
        
    }
}
