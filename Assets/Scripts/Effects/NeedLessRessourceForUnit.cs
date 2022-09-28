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
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    private int numberUnitCurrent;
    private int[] pivotRessourceList;
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
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
            }
        }
        
        else if (phase == EffectManager.enumEffectPhaseActivation.WhenThisUnitDie)
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
