using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class AleatInvokeMonster : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject cardUnit;
    [SerializeField] private int usingPhase = 3;
    [SerializeField] private List<Vector2> boardPosition = new List<Vector2>();
    private int random;
    private bool used;
    private bool here;
    private int i;
    

    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                InitArrayOfPosition();
                random = Random.Range(0, boardPosition.Count);
                
                for (int i = boardPosition.Count; i >= 0; i--)
                {
                    foreach (var unit in PlacementManager.instance.GetBoard())
                    {
                        foreach (var center in unit.emplacement)
                        {
                            if (center.Equals(boardPosition[random]))
                            {
                                here = true;
                            }
                        }
                    }

                    if (!here)
                    {
                        view.RPC("RPC_Action", RpcTarget.Others, boardPosition[random].x, boardPosition[random].y);
                        used = true;
                        EffectManager.instance.CancelSelection(1);
                        boardPosition.Clear();
                        return;    
                    }

                    boardPosition.RemoveAt(random);
                    random = Random.Range(0, boardPosition.Count);
                }
                
                boardPosition.Clear();
                EffectManager.instance.CancelSelection(1);
            }
        }
    }
    
    [PunRPC]
    private void RPC_Action(float x, float z)
    {
        PhotonNetwork.Instantiate(cardUnit.name, new Vector3(x,0.55f,z), PlayerSetup.instance.transform.rotation);
    }

    void InitArrayOfPosition()
    {
        for (float x = -3.5f ; x <= 3.5f; x++)
        {
            for (float y = -4.5f ; y <= 4.5; y++)
            {
                boardPosition.Add(new Vector2(x, y));
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
