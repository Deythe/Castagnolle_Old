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

    private int dmg=0;

    public void OnCast(EffectManager.enumEffectPhaseActivation phase)
    {
        if (view.AmOwner)
        {
            if (usingPhases.Contains(phase))
            {
                dmg = 0;
                for (int i = 0; i < DiceManager.instance.p_diceGauge.Length; i++)
                {
                    if (DiceManager.instance.p_diceGauge[i] == DiceListScriptable.enumRessources.Milk)
                    {
                        DiceManager.instance.p_diceGauge[i] = DiceListScriptable.enumRessources.Nothing;
                        DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                            DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, false, 0);
                        dmg++;
                    }
                }

                view.RPC("RPC_Action", RpcTarget.AllViaServer, EffectManager.instance.p_unitTarget1.GetComponent<PhotonView>().ViewID,
                    dmg);

                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CancelSelection();
                GetComponent<MonstreData>().p_model.layer = 6;
                used = true;
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action(int unit, int damage)
    {
        PlacementManager.instance.SearchMobWithID(unit).p_atk -= 2*damage;
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
