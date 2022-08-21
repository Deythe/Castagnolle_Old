using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DiceForAtk : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectPhaseActivation> usingPhase;
    [SerializeField] private List<EffectManager.enumConditionEffect> conditions;

    private bool used;

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        /*
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                for (int j = 0; j < DiceManager.instance.DiceChoosen.Length + DiceManager.instance.Gauge.Length; j++)
                {
                    if (j < DiceManager.instance.DiceChoosen.Length)
                    {
                        if (DiceManager.instance.DiceChoosen[j].Equals(4))
                        {
                            GetComponent<Monster>().p_attacked = false;
                            DiceManager.instance.DiceObjects[j].GetComponent<MeshRenderer>().enabled = false;
                            DiceManager.instance.DiceChoosen[j] = 0;
                            DeckManager.instance.CheckUnitWithRessources();
                            EffectManager.instance.CancelSelection(1);
                            GetComponent<Monster>().ChangeMeshRenderer(0);
                            used = true;
                            return;
                        }
                    }
                    else
                    {
                        if (DiceManager.instance.Gauge[j-DiceManager.instance.Gauge.Length].Equals(4))
                        {
                            GetComponent<Monster>().p_attacked = false;
                            DiceManager.instance.View.RPC("RPC_SynchGaugeDice",RpcTarget.All,  DiceManager.instance.DiceGaugeObjet[j-DiceManager.instance.Gauge.Length].GetComponent<PhotonView>().ViewID, false, 0);
                            DiceManager.instance.Gauge[j-DiceManager.instance.Gauge.Length] = 0;
                            DeckManager.instance.CheckUnitWithRessources();
                            EffectManager.instance.CancelSelection(1);
                            GetComponent<Monster>().ChangeMeshRenderer(0);
                            used = true;
                            return;
                        }
                    }
                }
                
                for (int j = 0; j < DiceManager.instance.DiceChoosen.Length + DiceManager.instance.Gauge.Length; j++)
                {
                    if (j < DiceManager.instance.DiceChoosen.Length)
                    {
                        if (DiceManager.instance.DiceChoosen[j]!=0)
                        {
                            GetComponent<Monster>().p_attacked = false;
                            DiceManager.instance.DiceObjects[j].GetComponent<MeshRenderer>().enabled = false;
                            DiceManager.instance.DiceChoosen[j] = 0;
                            DeckManager.instance.CheckUnitWithRessources();
                            EffectManager.instance.CancelSelection(1);
                            GetComponent<Monster>().ChangeMeshRenderer(0);
                            used = true;
                            return;
                        }
                    }
                    else
                    {
                        if (DiceManager.instance.Gauge[j-DiceManager.instance.Gauge.Length]!=0)
                        {
                            GetComponent<Monster>().p_attacked = false;
                            DiceManager.instance.View.RPC("RPC_SynchGaugeDice",RpcTarget.All,  DiceManager.instance.DiceGaugeObjet[j-DiceManager.instance.Gauge.Length].GetComponent<PhotonView>().ViewID, false, 0);
                            DiceManager.instance.Gauge[j-DiceManager.instance.Gauge.Length] = 0;
                            DeckManager.instance.CheckUnitWithRessources();
                            EffectManager.instance.CancelSelection(1);
                            GetComponent<Monster>().ChangeMeshRenderer(0);
                            used = true;
                            return;
                        }
                    }
                }
                
                EffectManager.instance.CancelSelection(1);
                UiManager.instance.ShowTextFeedBackWithDelay(3);
                GetComponent<Monster>().p_model.layer = 6;
            }
        }*/
    }
    List<EffectManager.enumEffectPhaseActivation> IEffects.GetPhaseActivation()
    {
        return usingPhase;
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
