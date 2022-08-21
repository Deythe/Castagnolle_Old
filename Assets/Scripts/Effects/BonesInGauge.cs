using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BonesInGauge : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private Texture2D bones;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhases;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;
    private bool used;
    
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

    List<EffectManager.enumEffectPhaseActivation> IEffects.GetPhaseActivation()
    {
        return usingPhases;
    }

    public List<EffectManager.enumConditionEffect> GetConditionsForActivation()
    {
        return conditions;
    }

    public bool GetUsed()
    {
        return used;
    }
    
    public void SetUsed(bool b)
    {
        used = b;
    }
}
