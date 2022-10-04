using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager instance;
    private GameObject goPrefabMonster;
    private GameObject currentUnit;
    private GameObject currentUnitPhoton;
    private bool isPlacing;
    private RaycastHit hit;
    private CardData currentCardSelection;

    [SerializeField] private PhotonView view;
    [SerializeField] private List<Case> board;
    [SerializeField] private bool haveAChampionOnBoard;
    [SerializeField] private List<Material> listMaterial;
    
    private float xAveragePosition, zAveragePosition;
    private bool isWaiting;

    [Serializable]
    public class Case{
        public GameObject monster;
        public List<Vector2> emplacement;
    }
    
    public PhotonView p_view
    {
        get => view;
    }
    
    public List<Case> p_board
    {
        get => board;
    }

    public List<Material> p_listMaterial
    {
        get => listMaterial;
    }

    public bool p_isWaiting
    {
        get => isWaiting;
    }

    public bool p_haveAChampionOnBoard
    {
        get => haveAChampionOnBoard;
    }
    public CardData p_currentCardSelection
    {
        get => currentCardSelection;
        set 
        {
            currentCardSelection = value;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (RoundManager.instance != null)
        {
            DragMonsterEmplacement();
        }
    }

    public void InstantiateCurrent(float x, float z )
    {
        currentUnit = Instantiate(goPrefabMonster,
            new Vector3(x, 0.55f, z) + PlayerSetup.instance.transform.forward,
                PlayerSetup.instance.transform.rotation);
        isPlacing = true;
    }
    
    public void ReInitMonsters()
    {
        foreach (var card in board)
        {
            if (card.monster.GetComponent<PhotonView>().AmOwner)
            {
                card.monster.GetComponent<MonstreData>().p_attacked = true;
                card.monster.GetComponent<MonstreData>().InitColorsTiles();
                
                if (card.monster.GetComponent<MonstreData>().p_isMovable)
                {
                    card.monster.GetComponent<MonstreData>().p_attacked = false;
                }
                
                card.monster.GetComponent<MonstreData>().ReloadEffect();
            }
        }
    }

    void DragMonsterEmplacement()
    {
        if (RoundManager.instance.p_roundState == RoundManager.enumRoundState.DragUnitPhase || (RoundManager.instance.p_roundState == RoundManager.enumRoundState.EffectPhase &&
            EffectManager.instance.p_specialInvocation))
        {
            if (Input.touchCount > 0)
            {
                if(!isWaiting){
                    
                    Physics.Raycast(PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position), out hit);
                    switch (Input.GetTouch(0).phase)
                    {
                        case TouchPhase.Began:
                            if (currentUnit != null)
                            {
                                InstantiateCurrent(hit.point.x, hit.point.z);
                            }

                            break;

                        case TouchPhase.Moved:
                            if (currentUnit == null)
                            {
                                InstantiateCurrent(hit.point.x, hit.point.z);
                            }

                            currentUnit.transform.position = new Vector3(hit.point.x, 0.55f, hit.point.z) +
                                                             PlayerSetup.instance.transform.forward;
                            break;

                        case TouchPhase.Ended:
                            case TouchPhase.Canceled:
                            if (isPlacing)
                            {
                                currentUnit.transform.position = new Vector3(
                                    Mathf.FloorToInt(currentUnit.transform.position.x) + 0.5f, 0.5f,
                                    Mathf.FloorToInt(currentUnit.transform.position.z) + 0.5f);

                                if (!CheckAlreadyHere(currentUnit) && CheckAllPosition(currentUnit))
                                {
                                    isPlacing = false;
                                    StartCoroutine(CoroutineSpawnMonster());
                                }
                                else
                                {
                                    Destroy(currentUnit);
                                    if (RoundManager.instance.p_roundState == RoundManager.enumRoundState.EffectPhase && EffectManager.instance.p_specialInvocation)
                                    {
                                        CancelSelection();
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    IEnumerator CoroutineSpawnMonster()
    {
        isWaiting = true;
        SoundManager.instance.PlaySFXSound(5, 0.05f);
        EffectManager.instance.View.RPC("RPC_PlayAnimation", RpcTarget.AllViaServer, 0,  AverageCenterX(currentUnit), 0.6f , AverageCenterZ(currentUnit), 4f);

        yield return new WaitForSeconds(1.2f);

        /*
        if (!EffectManager.instance.p_specialInvocation)
        {
            EffectManager.instance.ClearUnits();
            
            DiceManager.instance.DeleteAllResources(currentCardSelection.p_ressources);
            currentUnitPhoton = PhotonNetwork.Instantiate(goPrefabMonster.name,
                new Vector3(currentUnit.transform.position.x, 0.5f, currentUnit.transform.position.z),
                PlayerSetup.instance.transform.rotation, 0);

            Destroy(currentUnit);
            
            currentUnit = null;
            goPrefabMonster = null;
            currentCardSelection = null;

            if (!currentUnitPhoton.GetComponent<MonstreData>()
                .HaveAnEffectThisPhase(EffectManager.enumEffectPhaseActivation.WhenThisUnitIsInvoke))
            {
                RoundManager.instance.p_roundState = RoundManager.enumRoundState.DrawPhase;
            }
        }
        else
        {
            EffectManager.instance.ClearUnits();
            currentUnitPhoton = PhotonNetwork.Instantiate(goPrefabMonster.name,
                new Vector3(currentUnit.transform.position.x, 0.5f, currentUnit.transform.position.z),
                PlayerSetup.instance.transform.rotation, 0);
            
            Destroy(currentUnit);
            
            currentUnit = null;
            goPrefabMonster = null;
            currentCardSelection = null;

            if (!currentUnitPhoton.GetComponent<MonstreData>()
                .HaveAnEffectThisPhase(EffectManager.enumEffectPhaseActivation.WhenThisUnitIsInvoke))
            {
                RoundManager.instance.p_roundState = RoundManager.enumRoundState.DrawPhase;
            }
            
            if (obj.GetComponent<MonstreData>().HaveAnEffectThisPhase(EffectManager.enumEffectPhaseActivation.WhenThisUnitIsInvoke) && view.AmOwner && obj.GetComponent<MonstreData>().p_effect.GetIsActivable())
        {
            EffectManager.instance.p_currentUnit = obj;
            EffectManager.instance.UnitSelected(EffectManager.enumEffectPhaseActivation.WhenThisUnitIsInvoke);
        }
            
        }
        */
        currentUnitPhoton = null;
        isWaiting = false;
    }
    
    public void AddMonsterBoard(GameObject obj)
    {
        Case data = new Case();
        data.monster = obj;
        data.emplacement = new List<Vector2>();
        foreach (var center in obj.GetComponent<MonstreData>().GetCenters())
        {
            data.emplacement.Add(new Vector2(Mathf.FloorToInt(center.position.x) + 0.5f, Mathf.FloorToInt(center.position.z) + 0.5f));
        }
        board.Add(data);
        if (obj.GetComponent<MonstreData>().p_isChampion && obj.GetComponent<PhotonView>().AmOwner)
        {
            haveAChampionOnBoard = true;
        }

        EffectManager.instance.CheckAllHeroism();
    }

    public void RemoveMonsterBoard(int id)
    {
        for (int i = 0; i < board.Count; i++)
        {
            if (board[i].monster.GetComponent<MonstreData>().p_id == id)
            {
                if (board[i].monster.GetComponent<MonstreData>().p_isChampion &&
                    board[i].monster.GetComponent<PhotonView>().AmOwner)
                {
                    haveAChampionOnBoard = false;
                }
                
                board.Remove(board[i]);
                return;
            }
        }
        EffectManager.instance.CheckAllHeroism();
    }

    public float AverageCenterX(GameObject unit)
    {
        xAveragePosition = 0;
        foreach (var center in unit.GetComponent<MonstreData>().GetCenters())
        {
            xAveragePosition += center.position.x;
        }
        
        return xAveragePosition / unit.GetComponent<MonstreData>().GetCenters().Count;
    }

    public float AverageCenterZ(GameObject unit)
    {
        zAveragePosition = 0;
        foreach (var center in unit.GetComponent<MonstreData>().GetCenters())
        {
            zAveragePosition += center.position.z;
        }
        
        return zAveragePosition / unit.GetComponent<MonstreData>().GetCenters().Count;
    }

    public MonstreData SearchMobWithID(int unitID)
    {
        for (int i = 0; i < board.Count; i++)
        {
            if (board[i].monster.GetComponent<MonstreData>().p_id.Equals(unitID))
            {
                return board[i].monster.GetComponent<MonstreData>();
            }
        }

        return null;
    }

    public void CancelSelection()
    {
        ReInitPlacement();
        RoundManager.instance.p_roundState = RoundManager.enumRoundState.DrawPhase;
    }

    public void ReInitPlacement()
    {
        Destroy(currentUnit);
        goPrefabMonster = null;
        currentUnit = null;
        isPlacing = false;
    }

    public bool CheckAllPosition(GameObject obj)
    {
        foreach (var center in obj.GetComponent<MonstreData>().GetCenters())
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
            RoundManager.instance.p_localPlayerTurn==1)
        {
            return true;
        }

        if ((int)v.z + 1 == 5 &&
            RoundManager.instance.p_localPlayerTurn==2)
        {
            return true;
        }

        return CheckPositionBoard(new Vector2(Mathf.FloorToInt(v.x) + 0.5f, Mathf.FloorToInt(v.z) + 0.5f));
    }

    public bool CheckPositionBoard(Vector2 positionUnit)
    {
        foreach (Case data in board)
        {
            if (data.monster.GetComponent<PhotonView>().AmOwner)
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
        foreach (var tiles in obj.GetComponent<MonstreData>().GetCenters())
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
        float zcenter;
        if (RoundManager.instance.p_localPlayerTurn == 1)
        {
            zcenter = -10;
            foreach (var center in obj.GetComponent<MonstreData>().GetCenters())
            {
                if (center.position.z > zcenter)
                {
                    zcenter = center.position.z;
                }

            }
        }
        else
        {
            zcenter = 10;
            foreach (var center in obj.GetComponent<MonstreData>().GetCenters())
            {
                if (center.position.z < zcenter)
                {
                    zcenter = center.position.z;
                }
            }
        }

        return zcenter;
    }
    
        
    [PunRPC]
    private void RPC_ActivateEffect(int idUnit)
    {
        SearchMobWithID(idUnit).GetComponent<MonstreData>().ActivateEffects(0);
    }

    public GameObject GetPrefabUnit()
    {
        return goPrefabMonster;
    }
}
