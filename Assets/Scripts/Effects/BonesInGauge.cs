using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BonesInGauge : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases.Contains(phase))
            {
                if (DiceManager.instance != null)
                {
                    for (int i = 0; i < DiceManager.instance.p_diceGauge.Length; i++)
                    {
                        if (DiceManager.instance.p_diceGauge != null)
                        {
                            if (DiceManager.instance.p_diceGauge[i] == DiceListScriptable.enumRessources.Nothing)
                            {
                                DiceManager.instance.p_diceGauge[i] = DiceListScriptable.enumRessources.Milk;
                                DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                                    DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, true, DiceListScriptable.enumRessources.Milk);
                                used = true;
                                GetComponent<MonstreData>().p_model.layer = 6;
                                DeckManager.instance.CheckUnitWithRessources();
                                EffectManager.instance.CancelSelection();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = gameObject.GetPhotonView();
        usingPhases = new List<EffectManager.enumEffectPhaseActivation>(effectMother.GetUsingPhases());
        conditions = new List<EffectManager.enumConditionEffect>(effectMother.GetConditions());
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
