using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BonesForStrength : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;

    [SerializeField] private GameObject targetUnit;

    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        /*
        if (view.AmOwner)
        {
            if (phase == 3)
            {
                if (HaveABoneInGauge())
                {
                    RoundManager.instance.p_roundState = 7;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
                else
                {
                    EffectManager.instance.CancelSelection(1);
                    UiManager.instance.ShowTextFeedBackWithDelay(3);
                }
            }
            else if (phase == 7)
            {
                Debug.Log("CACA");
                targetUnit = EffectManager.instance.p_unitTarget1;

                view.RPC("RPC_Action", RpcTarget.AllViaServer,
                    EffectManager.instance.p_unitTarget1.GetComponent<Monster>().p_id);

                for (int i = 0; i < DiceManager.instance.Gauge.Length; i++)
                {
                    if (DiceManager.instance.Gauge[i].Equals(5))
                    {
                        DiceManager.instance.Gauge[i] = 0;
                        DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                            DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, 0);
                        EffectManager.instance.CancelSelection(1);
                        used = true;
                        GetComponent<Monster>().p_model.layer = 6;
                        DeckManager.instance.CheckUnitWithRessources();
                        return;
                    }
                }
            }
        }
        */
    }

    public bool HaveABoneInGauge()
    {
        foreach (var gaugedice in DiceManager.instance.p_diceGauge)
        {
            if (gaugedice.Equals(5))
            {
                return true;
            }
        }

        return false;
    }
    

    [PunRPC]
    private void RPC_Action(int unitID)
    {
        PlacementManager.instance.FindMobWithID(unitID).p_atk+=3;
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = effectMother.GetView();
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

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
}
