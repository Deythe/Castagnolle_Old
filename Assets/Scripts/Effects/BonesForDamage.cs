using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BonesForDamage : MonoBehaviour, IEffects
{
    
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    [SerializeField] private GameObject targetUnit;
    
    private int dmg=0;


    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        /*
        if (view.AmOwner)
        {
            if (HaveABoneInGauge())
            {
                if (phase == 3)
                {
                    RoundManager.instance.p_roundState = 6;
                    EffectManager.instance.CurrentUnit = gameObject;
                }
                else if (phase == 6)
                {
                    dmg = 0;
                    targetUnit = EffectManager.instance.p_unitTarget2;
                    
                    for (int i = 0; i < DiceManager.instance.Gauge.Length; i++)
                    {
                        if (DiceManager.instance.Gauge[i].Equals(5))
                        {
                            DiceManager.instance.Gauge[i] = 0;
                            DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                                DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, 0);
                            dmg++;
                        }
                    }

                    view.RPC("RPC_Action", RpcTarget.AllViaServer, targetUnit.GetComponent<PhotonView>().ViewID,
                        dmg);
                    
                    DeckManager.instance.CheckUnitWithRessources();
                    EffectManager.instance.CancelSelection(1);
                    GetComponent<Monster>().p_model.layer = 6;
                    used = true;
                }
            }
            else
            {
                EffectManager.instance.CancelSelection(1);
                UiManager.instance.ShowTextFeedBackWithDelay(3);
                GetComponent<Monster>().p_model.layer = 6;
            }
        }*/
    }
    
    public void TransferEffect(IEffects effectMother)
    {
        view = effectMother.GetView();
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
