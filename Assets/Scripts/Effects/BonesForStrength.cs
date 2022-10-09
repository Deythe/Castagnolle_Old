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

    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                view.RPC("RPC_Action", RpcTarget.AllViaServer,
                    EffectManager.instance.p_unitTarget1.GetComponent<MonstreData>().p_id);

                for (int i = 0; i < DiceManager.instance.p_diceGauge.Length; i++)
                {
                    if (DiceManager.instance.p_diceGauge[i] == DiceListScriptable.enumRessources.Milk)
                    {
                        DiceManager.instance.p_diceGauge[i] = DiceListScriptable.enumRessources.Nothing;
                        DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                            DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, DiceListScriptable.enumRessources.Nothing);
                        EffectManager.instance.CancelSelection();
                        EffectManager.instance.CheckAllHaveAMilkInGauge();
                        DeckManager.instance.CheckUnitWithRessources();
                        used = true;
                        GetComponent<MonstreData>().p_model.layer = 6;
                        return;
                    }
                }
            }
        }
        
    }
    

    [PunRPC]
    private void RPC_Action(int unitID)
    {
        PlacementManager.instance.FindMobWithID(unitID).p_atk+=3;
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
}
