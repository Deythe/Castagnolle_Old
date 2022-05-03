using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class InvokeMonsterActivable : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject cardInstance;
    [SerializeField] private Vector2[] positionChecking = new Vector2[4];
    private int usingPhase = 3;
    private bool used;
    
    public void OnCast(int phase)
    {
        if (phase == 5)
        {
            if (view.AmOwner)
            {
                List<Vector2> checks = positionChecking.ToList();

                do
                {
                    int t = Random.Range(0, checks.Count);
                    
                    if (!CheckCharactereHere(checks[t]))
                    {
                        object[] data = new object[] {cardInstance.GetComponent<CardData>().Atk, cardInstance.GetComponent<CardData>().IsChampion};
                        
                        PhotonNetwork.Instantiate(cardInstance.GetComponent<CardData>().Prefabs.name,
                            new Vector3(transform.position.x + checks[t].x, 0.55f, transform.position.z + checks[t].y),
                            PlayerSetup.instance.transform.rotation, 0, data);
                        checks.Clear();
                    }
                    else
                    {
                        checks.RemoveAt(t);
                    }
                } while (checks.Count != 0);

                EffectManager.instance.CancelSelection();
                used = true;
            }
        }
    }

    private bool CheckCharactereHere(Vector2 v)
    {
        foreach (var tiles in transform.GetComponent<Monster>().GetCenters())
        {
            if ((int)Mathf.Abs(tiles.position.x+v.x) > 3.5 || (int)Mathf.Abs(tiles.position.z+v.y) > 4.5)
            {
                return true;
            }
            
            foreach (var cases in PlacementManager.instance.GetBoard())
            {
                foreach (var empl in cases.emplacement)
                {
                    if (empl.x.Equals(tiles.position.x+v.x) &&
                        (empl.y.Equals(tiles.position.z+v.y)))
                    {
                        
                        return true;
                    }
                }
            }
        }

        return false;
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
