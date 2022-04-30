using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Evolve : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    
    [SerializeField] private Sprite evolve1;
    [SerializeField] private Sprite evolve2;
    
    [SerializeField] private GameObject unit1;
    [SerializeField] private GameObject unit2;
    
    private RectTransform cardListEvolve;
    private GameObject unitPivot;
    private int usingPhase = 1;
    private bool used;
    private int kill = 0;

    private void Start()
    {
        cardListEvolve = UiManager.instance.CarListChose;
        unitPivot = new GameObject();
    }


    public void OnCast(int phase)
    {
        if (view.AmOwner)
        {
            if (usingPhase == phase)
            {
                kill++;
                if (kill == 3)
                {
                    if (cardListEvolve.childCount==0)
                    {
                        EffectManager.instance.CurrentUnit = gameObject;

                        unitPivot.AddComponent<Image>().sprite = evolve1;
                        unitPivot.AddComponent<CardEvolve>().unit = unit1;
                        unitPivot = Instantiate(unitPivot, cardListEvolve);
                        unitPivot.GetComponent<RectTransform>().sizeDelta = new Vector3(300, 400);

                        unitPivot = new GameObject();
                        unitPivot.AddComponent<Image>().sprite = evolve2;
                        unitPivot.AddComponent<CardEvolve>().unit = unit2;
                        unitPivot = Instantiate(unitPivot, cardListEvolve);
                        unitPivot.GetComponent<RectTransform>().sizeDelta = new Vector3(300, 400);

                        unitPivot = null;
                    }
                    else
                    {
                        for (int i = cardListEvolve.childCount-1; i >= 0; i--)
                        {
                            cardListEvolve.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                    
                    RoundManager.instance.StateRound = 7;
                }
            }else if (phase == 7)
            {
                for (int i = cardListEvolve.childCount-1; i >= 0; i--)
                {
                    cardListEvolve.GetChild(i).gameObject.SetActive(false);
                }
                
                PhotonNetwork.Instantiate(EffectManager.instance.TargetUnit.name, transform.position,
                    PlayerSetup.instance.transform.rotation, 0);
                
                EffectManager.instance.CancelSelection();
                
                used = true;
                PhotonNetwork.Destroy(gameObject);
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
