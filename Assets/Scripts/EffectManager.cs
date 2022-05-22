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
    private RaycastHit hit;
    private int numberIdAll = 0;
    private int i, j;

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
        if (RoundManager.instance != null)
        {
            SelectTarget();
        }
    }

    void SelectTarget()
    {
        if ((RoundManager.instance.StateRound == 1 || RoundManager.instance.StateRound==6 || RoundManager.instance.StateRound ==7 ) && PlacementManager.instance.GetPrefabUnit() == null)
        {
            if (Input.touchCount > 0)
            {
                switch (Input.GetTouch(0).phase)
                {
                    case TouchPhase.Began:
                        Physics.Raycast(PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position), out hit);
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
    
    public bool CheckHeroism(Transform go, List<GameObject> mobNextTo, int numberCheck)
    {
        for (i = 0; i < numberCheck; i++)
        {
            for (j = 0; j < PlacementManager.instance.GetBoard().Count; j++)
            {
                foreach (var unitAlly in go.GetComponent<Monster>().GetCenters())
                {
                    if (!PlacementManager.instance.GetBoard()[j].monster.GetComponent<PhotonView>().AmOwner)
                    {
                        Debug.Log(PlacementManager.instance.GetBoard()[j].emplacement);
                        foreach (var center in PlacementManager.instance.GetBoard()[j].emplacement)
                        {
                            if (Vector2.Distance(center, new Vector2(unitAlly.position.x, unitAlly.position.z))
                                .Equals(1))
                            {
                                if (!mobNextTo.Contains(PlacementManager.instance.GetBoard()[j].monster))
                                {
                                    mobNextTo.Add(PlacementManager.instance.GetBoard()[j].monster);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (mobNextTo.Count.Equals(numberCheck))
        {
            mobNextTo.Clear();
            return true;
        }
        
        return false;   
        
    }
}
