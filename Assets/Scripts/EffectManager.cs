using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField] private GameObject currentUnit;
    [SerializeField] private GameObject targetUnit;
    private Ray ray;
    private RaycastHit hit;
    
    public GameObject CurrentUnit
    {
        get => currentUnit;
        set
        {
            currentUnit = value;
        }
    }

    public GameObject TargetUnit
    {
        get => targetUnit;
        set
        {
            targetUnit = value;
        }
    }
    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        SelectTarget();
    }

    void SelectTarget()
    {
        if ((RoundManager.instance.StateRound == 1 || RoundManager.instance.StateRound==6 || RoundManager.instance.StateRound ==7 ) && PlacementManager.instance.GetPrefabUnit() == null)
        {
            if (Input.touchCount > 0)
            {
                ray = PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position);
                Physics.Raycast(ray, out hit);
                
                switch (Input.GetTouch(0).phase)
                {
                    case TouchPhase.Began:
                        if (hit.collider != null)
                        {
                            Debug.Log(hit.collider.name);
                            if (hit.collider.GetComponent<Monster>() != null)
                            {
                                Debug.Log("Oui3");
                                switch (RoundManager.instance.StateRound)
                                {
                                    case 1:
                                        if (hit.collider.GetComponent<PhotonView>().AmOwner)
                                        {
                                            if (hit.collider.GetComponent<Monster>()
                                                .HaveAnEffectThisTurn(3))
                                            {
                                                RoundManager.instance.StateRound = 5;
                                                currentUnit = hit.collider.gameObject;
                                            }
                                        }
                                        break;
                                    case 6 :
                                        if (!hit.collider.GetComponent<PhotonView>().AmOwner)
                                        {
                                            targetUnit = hit.collider.gameObject;
                                            currentUnit.GetComponent<Monster>().ActivateEffects(RoundManager.instance.StateRound);
                                            CancelSelection();
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
    
    public void Action()
    {
        currentUnit.GetComponent<Monster>().ActivateEffects(5);
    }

    public void CancelSelection()
    {
        targetUnit = null;
        currentUnit = null;
        RoundManager.instance.StateRound = 1;
    }
}
