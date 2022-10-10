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
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                for (int j = 0; j < DiceManager.instance.p_diceChoosen.Length + DiceManager.instance.p_diceGauge.Length; j++)
                {
                    if (j < DiceManager.instance.p_diceChoosen.Length)
                    {
                        if (DiceManager.instance.p_diceChoosen[j] == DiceListScriptable.enumRessources.Neutral)
                        {
                            GetComponent<MonstreData>().p_attacked = false;
                            DiceManager.instance.p_diceObjects[j].GetComponent<MeshRenderer>().enabled = false;
                            DiceManager.instance.p_diceChoosen[j] = DiceListScriptable.enumRessources.Nothing;
                            GetComponent<MonstreData>().ChangeMeshRenderer(0);
                            GetComponent<MonstreData>().p_model.layer = 6;
                            used = true;
                            EffectManager.instance.CancelSelection();
                            DeckManager.instance.CheckUnitWithRessources();
                            return;
                        }
                    }
                    else
                    {
                        if (DiceManager.instance.p_diceGauge[j-DiceManager.instance.p_diceGauge.Length] == DiceListScriptable.enumRessources.Neutral)
                        {
                            GetComponent<MonstreData>().p_attacked = false;
                            DiceManager.instance.View.RPC("RPC_Synchp_diceGaugeDice",RpcTarget.All,  DiceManager.instance.p_diceObjects[j-DiceManager.instance.p_diceGauge.Length].GetComponent<PhotonView>().ViewID, false, DiceListScriptable.enumRessources.Nothing);
                            DiceManager.instance.p_diceGauge[j-DiceManager.instance.p_diceGauge.Length] = DiceListScriptable.enumRessources.Nothing;
                            GetComponent<MonstreData>().ChangeMeshRenderer(0);
                            used = true;
                            EffectManager.instance.CancelSelection();
                            DeckManager.instance.CheckUnitWithRessources();
                            return;
                        }
                    }
                }
                
                for (int j = 0; j < DiceManager.instance.p_diceChoosen.Length + DiceManager.instance.p_diceGauge.Length; j++)
                {
                    if (j < DiceManager.instance.p_diceChoosen.Length)
                    {
                        if (DiceManager.instance.p_diceChoosen[j]!= DiceListScriptable.enumRessources.Nothing)
                        {
                            GetComponent<MonstreData>().p_attacked = false;
                            DiceManager.instance.p_diceObjects[j].GetComponent<MeshRenderer>().enabled = false;
                            DiceManager.instance.p_diceChoosen[j] = DiceListScriptable.enumRessources.Nothing;
                            GetComponent<MonstreData>().ChangeMeshRenderer(0);
                            used = true;
                            EffectManager.instance.CancelSelection();
                            DeckManager.instance.CheckUnitWithRessources();
                            return;
                        }
                    }
                    else
                    {
                        if (DiceManager.instance.p_diceGauge[j-DiceManager.instance.p_diceGauge.Length]!= DiceListScriptable.enumRessources.Nothing)
                        {
                            GetComponent<MonstreData>().p_attacked = false;
                            DiceManager.instance.View.RPC("RPC_Synchp_diceGaugeDice",RpcTarget.All,  DiceManager.instance.p_diceObjects[j-DiceManager.instance.p_diceGauge.Length].GetComponent<PhotonView>().ViewID, false, DiceListScriptable.enumRessources.Nothing);
                            DiceManager.instance.p_diceGauge[j-DiceManager.instance.p_diceGauge.Length] = DiceListScriptable.enumRessources.Nothing;
                            GetComponent<MonstreData>().ChangeMeshRenderer(0);
                            used = true;
                            EffectManager.instance.CancelSelection();
                            DeckManager.instance.CheckUnitWithRessources();
                            return;
                        }
                    }
                }
            }
        }
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
    
    public void CancelEffect()
    {
        
    }
}
