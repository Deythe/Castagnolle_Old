using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DiceForAtk : MonoBehaviour, IEffects
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
    
    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
    }
    
    public void ResetEffect()
    {
        used = false;
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
