using Photon.Pun;
using UnityEngine;

public class Berserk : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private int usingPhase = 1;
    private bool used;
    private int numberPunch = 3;
    
    public void OnCast(int phase)
    {
        if (usingPhase == phase)
        {
            if (view.AmOwner)
            {
                if (numberPunch > 0)
                {
                    transform.GetComponent<Monster>().Attacked = false;
                    numberPunch--;
                    used = true;
                    used = false;
                }
                else
                {
                    numberPunch = 3;
                    used = true;
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
