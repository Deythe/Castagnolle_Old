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
    [SerializeField] private int numberPoupoul=1;
    [SerializeField] private GameObject unitPivot;
    private int random;
    private bool used;
    private bool here;
    private int i,j;
    

    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (phase == usingPhase)
            {
                for (j = 0; j < numberPoupoul; j++)
                {
                    InitArrayOfPosition();
                    Action();
                    boardPosition.Clear();
                }
                
                used = true;
                EffectManager.instance.CancelSelection(1);
            }
        }
    }

    private void Action()
    {
        for (i = boardPosition.Count; i > 0; i--)
        {
            here = false;
            random = Random.Range(0, boardPosition.Count);

            foreach (var place in PlacementManager.instance.GetBoard())
            {
                if (place.emplacement.Contains(boardPosition[random]))
                {
                    here = true;
                }
            }

            if (!here)
            {
                PhotonNetwork.Instantiate(cardUnit.name, new Vector3(boardPosition[random].x,0.5f,boardPosition[random].y), PlayerSetup.instance.transform.rotation, 0);
                return;
            }
            
            boardPosition.RemoveAt(random);
        }
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
