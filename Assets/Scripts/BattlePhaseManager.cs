using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class BattlePhaseManager : MonoBehaviour
{
    public static BattlePhaseManager instance;
    
    [SerializeField] private PhotonView playerView;
    [SerializeField] private float range=1;
    
    private Ray ray;
    private RaycastHit hit;
    
    private List<GameObject> unitsSelected;
    
    private List<Transform> deadUnitCenters;
    
    private GameObject unitTarget = null;
    private GameObject unitFusion;

    private int atkAlly;
    private bool isAttaking;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        unitsSelected = new List<GameObject>();
    }

    void Update()
    {
        CheckRaycast();
    }

    void CheckRaycast()
    {
        if (RoundManager.instance.GetStateRound() == 3 || RoundManager.instance.GetStateRound() == 4)
        {
            if (Input.touchCount > 0)
            {
                ray = PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position);
                Physics.Raycast(ray, out hit);

                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    if (hit.collider != null)
                    {
                        foreach (var unit in PlacementManager.instance.GetBoard())
                        {
                            foreach (var cases in unit.emplacement)
                            {
                                if (cases == new Vector2(Mathf.FloorToInt(hit.point.x) + 0.5f,
                                    Mathf.FloorToInt(hit.point.z) + 0.5f))
                                {
                                    if (RoundManager.instance.GetStateRound() == 3)
                                    {
                                        if (unit.monster.GetComponent<Monster>().GetView().IsMine && !unit.monster.GetComponent<Monster>().GetAttacked() &&
                                            !unitsSelected.Contains(unit.monster))
                                        {
                                            GameObject current = unit.monster;
                                            current.GetComponent<Monster>().BeChoosen();
                                            unitsSelected.Add(current);
                                        }
                                    }
                                    else if (RoundManager.instance.GetStateRound() == 4)
                                    {
                                        if (!unit.monster.GetComponent<Monster>().GetView().IsMine &&
                                            CheckInRange(unit.monster) && unitTarget == null)
                                        {
                                            unitTarget = unit.monster;
                                            unitTarget.GetComponent<Monster>().BeChoosen();
                                            isAttaking = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (unitsSelected.Count != 0)
                    {
                        CalculAlliesAtk();
                        RoundManager.instance.SetStateRound(4);
                    }
                }
            }
        }
    }

    public void Attack()
    {
        int result = atkAlly - unitTarget.GetComponent<Monster>().GetAtk();
        AllMonsterAttacked(true);
        
        switch (result)
        {
            case 0:
                playerView.RPC("RPC_Atk", RpcTarget.Others, unitTarget.GetComponent<PhotonView>().ViewID);
                
                LifeManager.instance.TakeDamageEnnemi(1);
                LifeManager.instance.TakeDamageHimself(1);
                
                foreach (var unit in unitsSelected)
                {
                    DeleteMonsterExten(unit);
                    PhotonNetwork.Destroy(unit);
                }
                break;
            
            case >0 :
                deadUnitCenters = unitTarget.GetComponent<Monster>().GetCenters();
                
                playerView.RPC("RPC_Atk", RpcTarget.Others, unitTarget.GetComponent<PhotonView>().ViewID);
                
                LifeManager.instance.TakeDamageEnnemi(AllCenterMoreFar());
                
                if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"]==1)
                {
                    AddAllExtension(StrongerMonster(), 1);
                }
                else
                {
                    AddAllExtension(StrongerMonster(), 2);
                }
                
                ActivateAllEffectInUnitSelected(1);
                
                break;
            
            case <0:
                LifeManager.instance.TakeDamageHimself(1);
                
                deadUnitCenters = unitsSelected[Random.Range(0, unitsSelected.Count)].GetComponent<Monster>().GetCenters();
                
                foreach (var unit in unitsSelected)
                {
                    DeleteMonsterExten(unit);
                    PhotonNetwork.Destroy(unit);
                }
                
                if ((int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"]==1)
                {
                    AddAllExtension(unitTarget, 1);
                }
                else
                {
                    AddAllExtension(unitTarget, 2);
                }

                break;
        }

        RoundManager.instance.SetStateRound(3);
        isAttaking = false;
        
        ClearUnits();
    }

    public void AllMonsterAttacked(bool b)
    {
        foreach (var unit in unitsSelected)
        {
            unit.GetComponent<Monster>().SetAttacked(b);
        }
    }

    private GameObject StrongerMonster()
    {
        GameObject unitSelected = unitsSelected[0];
        
        foreach (var unit in unitsSelected)
        {
            if (unit.GetComponent<Monster>().GetAtk() > unitSelected.GetComponent<Monster>().GetAtk())
            {
                unitSelected = unit;
            }
        }

        return unitSelected;
    }

    private void AddExtension(GameObject unitMore, int owner)
    {
        for (int j = 0; j < unitMore.GetComponent<Monster>().GetCenters().Count; j++)
        {
            foreach (var centerUnitDead in deadUnitCenters)
            {
                if (Vector3.Distance(centerUnitDead.position,
                    unitMore.GetComponent<Monster>().GetCenters()[j].position).Equals(range))
                {
                    unitFusion = PhotonNetwork.Instantiate("Tile", centerUnitDead.position,
                        PlayerSetup.instance.transform.rotation, 0);

                    playerView.RPC("RPC_SyncUnit", RpcTarget.All, unitFusion.GetComponent<UnitExtension>().GetView().ViewID, unitMore.GetComponent<Monster>().GetView().ViewID, owner );

                    return;
                }
            }
        }
    }

    private void AddAllExtension(GameObject unitMore, int owner)
    {
        for (int i = 0; i < Mathf.Ceil(deadUnitCenters.Count/2f); i++)
        {
            AddExtension(unitMore, owner);
        }
        
        deadUnitCenters.Clear();
    }

    public float AllCenterMoreFar()
    {
        float farCenter = -10;
        foreach (var unit in unitsSelected)
        {
            if (PlacementManager.instance.CenterMoreFar(unit) > farCenter)
            {
                farCenter = PlacementManager.instance.CenterMoreFar(unit);
            }
        }

        return farCenter;
    }

    public bool CheckInRange(GameObject v)
    {
        foreach (var unit in unitsSelected)
        {
            bool unitCheck = false;
            
            foreach (var center in unit.GetComponent<Monster>().GetCenters())
            {
                foreach (var targetCenter in v.GetComponent<Monster>().GetCenters())
                {
                    if (Vector3.Distance(center.position, targetCenter.position).Equals(range))
                    {
                        unitCheck = true;
                    }
                }
            }

            if (!unitCheck)
            {
                return false;
            }
        }
        
        return true;
    }

    public void CancelSelection()
    {
        ClearUnits();
        isAttaking = false;
        RoundManager.instance.SetStateRound(3);
    }

    public void ClearUnits()
    {
        for (int i = 0; i < unitsSelected.Count; i++)
        {
            unitsSelected[i].GetComponent<Monster>().NotChossen();
        }
        
        unitsSelected.Clear();

        if (unitTarget != null)
        {
            unitTarget.GetComponent<Monster>().NotChossen();
            unitTarget = null;
        }
    }

    public void CalculAlliesAtk()
    {
        atkAlly = 0;
        foreach (var unit in unitsSelected)
        {
            atkAlly += unit.GetComponent<Monster>().GetAtk();
        }
    }

    private void ActivateAllEffectInUnitSelected(int i)
    {
        foreach (var unit in unitsSelected)
        {
            unit.GetComponent<Monster>().ActivateEffects(i);
        }
    }
    

    public bool GetIsAttacking()
    {
        return isAttaking;
    }
    
    public void SetItAttacking(bool i)
    {
        isAttaking = i;
    }

    public void DeleteMonsterExten(GameObject unit)
    {
        for (int i = unit.GetComponent<Monster>().GetExtention().Count-1; i >= 0; i--)
        {
            PhotonNetwork.Destroy(unit.GetComponent<Monster>().GetExtention()[i]);
        }
    }
    
    [PunRPC]
    private void RPC_SyncUnit(int idExten, int idMore, int owner)
    {
        PhotonView viewExten = PhotonView.Find(idExten);
        viewExten.TransferOwnership(owner);
        
        PhotonView viewMore = PhotonView.Find(idMore);
        
        viewMore.gameObject.GetComponent<Monster>().GetExtention().Add(viewExten.gameObject);
        
        viewExten.gameObject.GetComponent<UnitExtension>().Init(viewMore.gameObject);
    }
    
    [PunRPC]
    private void RPC_Atk(int id)
    {
        PhotonView ph = PhotonView.Find(id);
        if (ph.IsMine)
        {
            DeleteMonsterExten(ph.gameObject);
            PhotonNetwork.Destroy(ph.gameObject);
        }
    }
    
}
