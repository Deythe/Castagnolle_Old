using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class AleatInvokeMonster : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    
    [SerializeField] private GameObject cardUnit;
    [SerializeField] private List<Vector2> boardPosition = new List<Vector2>();
    [SerializeField] private int numberUnitToInvoke = 1;
    [SerializeField] private GameObject unitPivot;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;
    private List<GameObject> listEffectWaiting;
    private int random;
    private bool here, done;
    private int i, j;


    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                StartCoroutine(CoroutineOnCast());
            }
        }
    }

    IEnumerator CoroutineOnCast()
    {
        CheckPlayerInitArrayOfPosition();
        for (j = 0; j < numberUnitToInvoke; j++)
        {
            Action();
            yield return new WaitUntil(() => done);
            done = false;
        }

        boardPosition.Clear();
        used = true;
        EffectManager.instance.CancelSelection();
        GetComponent<MonstreData>().p_model.layer = 6;
        
        for (j = 0; j < listEffectWaiting.Count; j++)
        {
            if (listEffectWaiting[j].GetComponent<MonstreData>()
                .HaveAnEffectThisPhase(EffectManager.enumEffectConditionActivation.WhenThisUnitIsInvoke))
            {
                EffectManager.instance.p_currentUnit = listEffectWaiting[j];
                EffectManager.instance.UnitSelected(EffectManager.enumEffectConditionActivation.WhenThisUnitIsInvoke);
                yield return new WaitUntil(() => EffectManager.instance.p_currentUnit == null);
            }

            done = false;
        }
        
    }

    void Action()
    {
        {
            here = false;
            random = Random.Range(0, boardPosition.Count);

            foreach (var place in PlacementManager.instance.p_board)
            {
                if (place.emplacement.Contains(boardPosition[random]))
                {
                    here = true;
                }
            }

            if (!here)
            {
                StartCoroutine(CoroutineInvoke());
                return;
            }
            
            boardPosition.RemoveAt(random);
            Action();
        }
    }

    IEnumerator CoroutineInvoke()
    {
        unitPivot = Instantiate(cardUnit.GetComponent<CardData>().p_prefabs,
            new Vector3(boardPosition[random].x, 0.5f, boardPosition[random].y),
            PlayerSetup.instance.transform.rotation);
        EffectManager.instance.View.RPC("RPC_PlayAnimation", RpcTarget.AllViaServer, 0,  PlacementManager.instance.AverageCenterX(unitPivot), 0.6f , PlacementManager.instance.AverageCenterZ(unitPivot), 4f);
        
        yield return new WaitForSeconds(1.2f);
        
        Destroy(unitPivot);
        unitPivot = PhotonNetwork.Instantiate(cardUnit.GetComponent<CardData>().p_prefabs.name,
            new Vector3(boardPosition[random].x, 0.5f, boardPosition[random].y),
            PlayerSetup.instance.transform.rotation, 0);
        listEffectWaiting.Add(unitPivot);
        unitPivot = null;
        done = true;
    }
    
    void CheckPlayerInitArrayOfPosition()
    {
        if (RoundManager.instance.p_localPlayerTurn==1)
        {
            for (float y = -4.5f; y <= 0; y++)
            {
                InitArrayOfPosition(y);
            }
        }
        else
        {
            for (float y = 4.5f; y >= 0; y--)
            {
                InitArrayOfPosition(y);
            }
        }
    }

    void InitArrayOfPosition(float y)
    {
        for (float x = -3.5f; x <= 3.5f; x++)
        {
            boardPosition.Add(new Vector2(x, y));
        }
    }

    public void TransferEffect(IEffects effectMother)
    {
        AleatInvokeMonster pivot = effectMother as AleatInvokeMonster;
        view = gameObject.GetPhotonView();
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();

        cardUnit = pivot.cardUnit;
        numberUnitToInvoke = pivot.numberUnitToInvoke;
        listEffectWaiting = new List<GameObject>();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectConditionActivation> GetConditions()
    {
        return conditions;
    }
    
    public List<EffectManager.enumActionEffect> GetActions()
    {
        return actions;
    }
    
    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
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