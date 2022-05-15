using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    [SerializeField] private GameObject targetUnitAlly;
    [SerializeField] private GameObject currentUnit;
    [SerializeField] private GameObject targetUnitEnnemi;
    private Ray ray;
    private int numberIdAll = 0;
    private RaycastHit hit;

    public GameObject AllieUnit
    {
        get => targetUnitAlly;
        set
        {
            targetUnitAlly = value;
        }
    }

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
        get => targetUnitEnnemi;
        set
        {
            targetUnitEnnemi = value;
        }
    }
    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        SelectTarget();
        //ActivateAllEffectWhenUnitDie();
    }

    public void ActivateAllEffectWhenUnitDie()
    {
        if (numberIdAll < PlacementManager.instance.GetBoard().Count)
        {
            Debug.Log("Unit invoqué");
            numberIdAll = PlacementManager.instance.GetBoard().Count;
        }else if (numberIdAll > PlacementManager.instance.GetBoard().Count)
        {
            Debug.Log("Unit morte");
            numberIdAll = PlacementManager.instance.GetBoard().Count;
            
            foreach (var cases in PlacementManager.instance.GetBoard())
            {
                if (cases.monster.GetComponent<Monster>().HaveAnEffectThisTurn(5))
                {
                    cases.monster.GetComponent<Monster>().ActivateEffects(5);
                }
            }    
        }
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
                            if (hit.collider.GetComponent<Monster>() != null)
                            {
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
                                            Debug.Log("Enemi");
                                            targetUnitEnnemi = hit.collider.gameObject;
                                            currentUnit.GetComponent<Monster>().ActivateEffects(RoundManager.instance.StateRound);
                                        }
                                        break;
                                    case 7 :
                                        if (hit.collider.GetComponent<PhotonView>().AmOwner)
                                        {
                                            Debug.Log("Ally");
                                            targetUnitAlly = hit.collider.gameObject;
                                            currentUnit.GetComponent<Monster>().ActivateEffects(RoundManager.instance.StateRound);
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
        currentUnit.GetComponent<Monster>().ActivateEffects(3);
    }

    public void CancelSelection(int state)
    {
        targetUnitAlly = null;
        targetUnitEnnemi = null;
        currentUnit = null;
        RoundManager.instance.StateRound = state;
    }
}
