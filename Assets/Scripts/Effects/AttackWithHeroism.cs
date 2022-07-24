using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackWithHeroism : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private Monster targetUnit;
    [SerializeField] private int usingPhase = 3;
    [SerializeField] private List<GameObject> mobNextTo;
    [SerializeField] private int heroism;
    private bool used;


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                if (EffectManager.instance.CheckHeroism(transform, mobNextTo, heroism))
                {
                    GetComponent<Monster>().p_attacked = false;
                    used = true;
                    EffectManager.instance.CancelSelection(1);
                    GetComponent<Monster>().p_model.layer = 6;
                    GetComponent<Monster>().ChangeMeshRenderer(0);
                }
                else
                {
                    EffectManager.instance.CancelSelection(1);
                    UiManager.instance.ShowTextFeedBackWithDelay(3);
                    GetComponent<Monster>().p_model.layer = 6;
                }
            }
        }
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
