using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager instance;

    private GameObject goPrefabMonster;
    private GameObject currentUnit;

    private bool isPlacing;
    private Ray ray;
    private RaycastHit hit;

    [SerializeField] private List<Case> board;

    [Serializable]
    public class Case{
        public GameObject monster;
        public List<Vector2> emplacement;
    }

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        CheckRaycast();
        ShowMonsterEmplacement();
    }

    void CheckRaycast()
    {
        if (RoundManager.instance.GetStateRound()==2)
        {
            if (Input.touchCount>0)
            {
                ray = PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position);
                Physics.Raycast(ray, out hit);
            }
        }
    }

    void ShowMonsterEmplacement()
    {
        if (RoundManager.instance.GetStateRound() == 2)
        {
            if (Input.touchCount > 0)
            {
                switch (Input.GetTouch(0).phase)
                {
                    case TouchPhase.Began:
                        currentUnit = Instantiate(goPrefabMonster, new Vector3(hit.point.x, 0.55f, hit.point.z)+PlayerSetup.instance.transform.forward,
                            PlayerSetup.instance.transform.rotation);

                        isPlacing = true;
                        break;
                    
                    case TouchPhase.Moved:
                        currentUnit.transform.position = new Vector3(hit.point.x, 0.55f, hit.point.z)+PlayerSetup.instance.transform.forward;
                        break;
                    
                    case TouchPhase.Ended:
                        if (isPlacing)
                        {
                            currentUnit.transform.position = new Vector3(
                                Mathf.FloorToInt(currentUnit.transform.position.x) + 0.5f, 0.55f,
                                Mathf.FloorToInt(currentUnit.transform.position.z) + 0.5f);
                            
                            if (!CheckAlreadyHere(currentUnit) && CheckAllPosition(currentUnit))
                            {
                                PhotonNetwork.Instantiate(goPrefabMonster.name, currentUnit.transform.position,
                                    PlayerSetup.instance.transform.rotation, 0);
                                
                                Destroy(currentUnit);
                                RoundManager.instance.SetStateRound(1);
                                
                                DiceManager.instance.DeleteAllResources(currentUnit.GetComponent<Monster>().GetStat().resources);
                                
                                goPrefabMonster = null;
                                currentUnit = null;
                                isPlacing = false;
                            }
                            else
                            {
                                Destroy(currentUnit);
                            }
                                
                        }
                        break;
                }
            }
        }
    }

    public void CancelSelection()
    {
        Destroy(currentUnit);
        RoundManager.instance.SetStateRound(1);
        goPrefabMonster = null;
        currentUnit = null;
        isPlacing = false;
    }
    
    
    public bool CheckAllPosition(GameObject obj)
    {
        foreach (var center in obj.GetComponent<Monster>().GetCenters())
        {
            if (CheckLinkWithOthers(center.position))
            {
                return true;
            }
        }
        
        return false;
    }

    public void SetGOPrefabsMonster(GameObject go)
    {
        goPrefabMonster = go;
    }

    public bool CheckLinkWithOthers(Vector3 v)
    {
        if (Mathf.FloorToInt(v.z) == -5 &&
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] == 1)
        {
            return true;
        }

        if ((int)v.z + 1 == 5 &&
            (int) PhotonNetwork.LocalPlayer.CustomProperties["RoundNumber"] == 2)
        {
            return true;
        }

        return CheckPositionBoard(new Vector2(Mathf.FloorToInt(v.x) + 0.5f, Mathf.FloorToInt(v.z) + 0.5f));
    }

    public bool CheckPositionBoard(Vector2 positionUnit)
    {
        foreach (var data in board)
        {
            if (data.monster.GetComponent<Monster>().GetOwner() ==
                (int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"])
            {
                foreach (var vector in data.emplacement)
                {
                    if ((vector.x.Equals(positionUnit.x + 1) || vector.x.Equals(positionUnit.x - 1)) && vector.y.Equals(positionUnit.y))
                    {
                        return true;
                    }
                    if ((vector.y.Equals(positionUnit.y + 1) || vector.y.Equals(positionUnit.y - 1)) && vector.x.Equals(positionUnit.x))
                    {
                        return true;
                    }   
                }
            }
        }
        
        return false;
    }

    bool CheckAlreadyHere(GameObject obj)
    {
        foreach (var tiles in obj.GetComponent<Monster>().GetCenters())
        {
            if ((int)Mathf.Abs(tiles.position.x) > 3.5 || (int)Mathf.Abs(tiles.position.z) > 4.5)
            {
                return true;
            }
            
            foreach (var data in board)
            {
                foreach (var vector in data.emplacement)
                {
                    if (vector.x.Equals(tiles.position.x) &&
                        vector.y.Equals(tiles.position.z))
                    {
                        return true;
                    }   
                }
            }   
        }

        return false;
    }
    
    public void AddMonsterBoard(GameObject obj)
    {
        Case data = new Case();
        data.monster = obj;
        data.emplacement = new List<Vector2>();
        foreach (var center in obj.GetComponent<Monster>().GetCenters())
        {
            data.emplacement.Add(new Vector2(Mathf.FloorToInt(center.position.x) + 0.5f, Mathf.FloorToInt(center.position.z) + 0.5f));
        }
        board.Add(data);
    }
    
    public void AddExtentionMonsterBoard(GameObject exten, GameObject mother)
    {
        foreach (var unit in board)
        {
            if (unit.monster.Equals(mother))
            {
                 unit.emplacement.Add(new Vector2(Mathf.FloorToInt(exten.transform.position.x) + 0.5f,
                    Mathf.FloorToInt(exten.transform.position.z) + 0.5f));
            }
        }
    }
    
    public float CenterMoreFar(GameObject obj)
    {
        float zcenter=-10;
        foreach (var center in obj.GetComponent<Monster>().GetCenters())
        {
            if (center.position.z > zcenter)
            {
                zcenter = center.position.z;
            }
        }

        return zcenter;
    }

    public List<Case> GetBoard()
    {
        return board;
    }
    
    

}
