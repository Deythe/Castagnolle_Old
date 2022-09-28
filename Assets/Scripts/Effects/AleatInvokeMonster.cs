using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class AleatInvokeMonster : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    
    [SerializeField] private GameObject cardUnit;
    [SerializeField] private List<Vector2> boardPosition = new List<Vector2>();
    [SerializeField] private int numberPoupoul = 1;
    [SerializeField] private GameObject unitPivot;

    private int random;
    private bool here;
    private int i, j;


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases[0].Equals(phase))
            {
                for (j = 0; j < numberPoupoul; j++)
                    if (RoundManager.instance.p_localPlayerTurn == 1)
                    {
                        InitArrayOfPositionForJ1();
                    }
                    else
                    {
                        InitArrayOfPositionForJ2();
                    }

                Action();
                boardPosition.Clear();
                used = true;
                EffectManager.instance.CancelSelection(RoundManager.enumRoundState.DrawPhase);
                GetComponent<MonstreData>().p_model.layer = 6;
            }
        }
    }

    private void Action()
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
                unitPivot = PhotonNetwork.Instantiate(cardUnit.name,
                    new Vector3(boardPosition[random].x, 0.5f, boardPosition[random].y),
                    PlayerSetup.instance.transform.rotation, 0);
                unitPivot.GetComponent<MonstreData>().ActivateEffects(0);
                return;
            }

            boardPosition.RemoveAt(random);
        }
    }

    void InitArrayOfPositionForJ1()
    {
        for (float y = -4.5f; y <= 0; y++)
        {
            for (float x = -3.5f; x <= 3.5f; x++)
            {
                boardPosition.Add(new Vector2(x, y));
            }
        }
    }

    void InitArrayOfPositionForJ2()
    {
        for (float y = 4.5f; y >= 0; y--)
        {
            for (float x = -3.5f; x <= 3.5f; x++)
            {
                boardPosition.Add(new Vector2(x, y));
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