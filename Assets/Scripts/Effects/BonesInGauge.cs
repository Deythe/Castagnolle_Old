using Photon.Pun;
using UnityEngine;

public class BonesInGauge : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private Texture2D bones;
    private int usingPhase = 2;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                for (int i = 0; i < DiceManager.instance.Gauge.Length; i++)
                {
                    if (DiceManager.instance.Gauge[i].Equals(0))
                    {
                        Debug.Log(bones.format);
                        byte[] bytes = bones.GetRawTextureData();
                        DiceManager.instance.Gauge[i] = 5;
                        DiceManager.instance.View.RPC("RPC_SynchGaugeDice",RpcTarget.All, DiceManager.instance.DiceGaugeObjet[i].GetComponent<PhotonView>().ViewID, true, bytes);
                        used = true;
                        return;
                    }
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