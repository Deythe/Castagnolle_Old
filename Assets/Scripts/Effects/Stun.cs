using Photon.Pun;
using UnityEngine;

public class Stun : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject targetUnit;
    [SerializeField] private int usingPhase = 0;
    private bool used;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                RoundManager.instance.StateRound = 6;
                EffectManager.instance.CurrentUnit = gameObject;
            }
            else if (phase == 6)
            {
                view.RPC("RPC_Action", RpcTarget.AllViaServer, EffectManager.instance.TargetUnit.GetComponent<PhotonView>().ViewID);
                used = true;
                EffectManager.instance.CancelSelection(1);
            }
        }

        if (phase == 2)
        {
            targetUnit.GetComponent<Monster>().p_isMovable = false;
            targetUnit.GetComponent<Monster>().Attacked = false;
        }
    }
    
    [PunRPC]
    private void RPC_Action(int idTarget)
    {
        targetUnit = PlacementManager.instance.SearchMobWithID(idTarget).gameObject;
        targetUnit.GetComponent<Monster>().p_isMovable = false;
        targetUnit.GetComponent<Monster>().Attacked = true;
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
