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
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;

    private int numberUnitCurrent;
    private int[] pivotRessourceList;
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
                    if (card.GetComponent<CardData>().p_prefabs.Equals(originalCard.GetComponent<CardData>().p_prefabs))
                    {
                        if (card.GetComponent<CardData>().p_ressources.Count > 0)
                        {
                            card.GetComponent<CardData>().p_ressources
                                .RemoveAt(card.GetComponent<CardData>().p_ressources.Count - 1);
                        }
                    }
                }
                
                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CancelSelection();
            }
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
                        if (card.GetComponent<CardData>().p_prefabs.Equals(originalCard.GetComponent<CardData>().p_prefabs))
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
                        if (PlacementManager.instance.p_board[numberUnitCurrent - 1].monster.Equals(originalCard.GetComponent<CardData>().p_prefabs))
                        {
                            motherUnit = null;
                            unitOnBoard.Clear();
                            ResetUnit();
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
            if (card.GetComponent<CardData>().p_prefabs.Equals(originalCard.GetComponent<CardData>().p_prefabs))
            {
                card.GetComponent<CardData>().p_ressources.Clear();
                card.GetComponent<CardData>().p_ressources = originalCard.GetComponent<CardData>().p_ressources;
            }
        }
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        NeedLessRessourceForUnit pivot = effectMother as NeedLessRessourceForUnit;
        view = gameObject.GetPhotonView();
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();

        originalCard = pivot.originalCard;
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
}
