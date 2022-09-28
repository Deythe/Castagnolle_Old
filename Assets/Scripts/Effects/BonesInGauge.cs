using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BonesInGauge : MonoBehaviour, IEffects
{
    public List<EffectManager.enumEffectPhaseActivation> test;
    
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;

    [SerializeField] private Texture2D bones;
    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        /*
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                if (DiceManager.instance != null)
                {
                    for (int i = 0; i < DiceManager.instance.Gauge.Length; i++)
                    {
                        if (DiceManager.instance.Gauge != null)
                        {
                            if (DiceManager.instance.Gauge[i].Equals(0))
                            {
                                DiceManager.instance.Gauge[i] = 5;
                                DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                                    DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, true, 5);
                                used = true;
                                GetComponent<Monster>().p_model.layer = 6;
                                DeckManager.instance.CheckUnitWithRessources();
                                return;
                            }
                        }
                    }
                }
            }
        }*/
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
