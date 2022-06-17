using Photon.Pun;
using UnityEngine;

public class DiceForAtk : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 3;
    private bool used;

    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                if (HaveDice() != -1)
                {
                    GetComponent<Monster>().Attacked = false;
                    DiceManager.instance.DiceObjects[HaveDice()].GetComponent<MeshRenderer>().enabled = false;
                    DiceManager.instance.DiceChoosen[HaveDice()] = 0;
                    DeckManager.instance.CheckUnitWithRessources();
                    EffectManager.instance.CancelSelection(1);
                    GetComponent<Monster>().ChangeMeshRenderer(0);
                    used = true;
                }else if (HaveDiceInGauge()!=-1)
                {
                    GetComponent<Monster>().Attacked = false;
                    DiceManager.instance.View.RPC("RPC_SynchGaugeDice",RpcTarget.All,  DiceManager.instance.DiceGaugeObjet[HaveDiceInGauge()].GetComponent<PhotonView>().ViewID, false, 0);
                    DiceManager.instance.DiceChoosen[HaveDiceInGauge()] = 0;
                    DeckManager.instance.CheckUnitWithRessources();
                    EffectManager.instance.CancelSelection(1);
                    GetComponent<Monster>().ChangeMeshRenderer(0);
                    used = true;
                }
                else
                {
                    EffectManager.instance.CancelSelection(1);
                    UiManager.instance.ShowTextFeedBackWithDelay(3);
                }
                
                GetComponent<Monster>().p_model.layer = 6;
            }
        }
    }

    int HaveDice()
    {
        for (int i = 0; i < DiceManager.instance.DiceChoosen.Length; i++)
        {
            if (DiceManager.instance.DiceChoosen[i]!=0)
            {
                return i;
            }
        }
        
        return -1;
    }

    int HaveDiceInGauge()
    {
        for (int j = 0; j < DiceManager.instance.Gauge.Length; j++)
        {
            if (DiceManager.instance.Gauge[j]!=0)
            {
                return j;
            }
        }

        return -1;
    }

    public int GetPhaseActivation()
    {
        return usingPhase;
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
