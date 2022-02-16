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
    private GameObject unitTarget;

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
        if (RoundManager.instance.GetStateRound()==3 || RoundManager.instance.GetStateRound()==4)
        {
            if (Input.touchCount > 0)
            {
                ray = PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position);

                Physics.Raycast(ray, out hit);

                switch (Input.GetTouch(0).phase)
                {
                    case TouchPhase.Began:
                        if (hit.collider != null && hit.collider.GetComponent<PlatformMonster>() != null)
                        {
                            if (hit.collider.GetComponent<PlatformMonster>().GetOwner() ==
                                (int) PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"])
                            {
                                if (RoundManager.instance.GetStateRound() == 3)
                                {
                                    if (!unitsSelected.Contains(hit.collider.gameObject))
                                    {
                                        GameObject current = hit.collider.gameObject;
                                        current.GetComponent<PlatformMonster>().BeChoosen();
                                        unitsSelected.Add(current);
                                    }
                                }
                            }

                            else
                            {
                                if (RoundManager.instance.GetStateRound() == 4)
                                {
                                    if (CheckInRange(unitsSelected[0], hit.collider.gameObject))
                                    {
                                        unitTarget = hit.collider.gameObject;
                                        unitTarget.GetComponent<PlatformMonster>().BeChoosen();
                                        isAttaking = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Trop loins");
                                    }
                                }
                            }
                        }

                        break;

                    case TouchPhase.Ended:
                        if (unitsSelected.Count != 0)
                        {
                            CalculAlliesAtk();
                            RoundManager.instance.SetStateRound(4);
                        }
                        break;
                }
            }
        }
    }

    public void Attack()
    {
        int result = atkAlly - unitTarget.GetComponent<PlatformMonster>().GetAtk();
        switch (result)
        {
            case 0:
                playerView.RPC("RPC_Atk", RpcTarget.Others, unitTarget.GetComponent<PhotonView>().ViewID);
                foreach (var unit in unitsSelected)
                {
                    PhotonNetwork.Destroy(unit);
                }
                //(int)Mathf.Abs(current.transform.position.z)+1 = zone
                LifeManager.instance.TakeDamageEnnemi(Mathf.Abs(1));
                LifeManager.instance.TakeDamageHimself(Mathf.Abs(1));
                break;
            case >0 :
                playerView.RPC("RPC_Atk", RpcTarget.Others, unitTarget.GetComponent<PhotonView>().ViewID);
                LifeManager.instance.TakeDamageEnnemi(Mathf.Abs(1));
                break;
            
            case <0:
                LifeManager.instance.TakeDamageHimself(Mathf.Abs(1));
                foreach (var unit in unitsSelected)
                {
                    PhotonNetwork.Destroy(unit);
                }

                break;
        }
        
        RoundManager.instance.SetStateRound(3);
        isAttaking = false;
        ClearUnits();
    }

    public bool CheckInRange(GameObject obj, GameObject v)
    {
        foreach (var center in obj.GetComponent<PlatformMonster>().GetCenters())
        {
            foreach (var targetCenter in v.GetComponent<PlatformMonster>().GetCenters())
            {
                if (Vector3.Distance(center.position, targetCenter.position).Equals(range))
                {
                    return true;
                }    
            }
        }
        return false;
    }

    public void CancelSelection()
    {
        ClearUnits();
        RoundManager.instance.SetStateRound(3);
    }

    public void ClearUnits()
    {
        for (int i = 0; i < unitsSelected.Count; i++)
        {
            unitsSelected[i].GetComponent<PlatformMonster>().NotChossen();
        }
        unitsSelected.Clear();

        if (unitTarget != null)
        {
            unitTarget.GetComponent<PlatformMonster>().NotChossen();
            unitTarget = null;
        }
    }

    public void CalculAlliesAtk()
    {
        atkAlly = 0;
        foreach (var unit in unitsSelected)
        {
            atkAlly += unit.GetComponent<PlatformMonster>().GetAtk();
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
    
    [PunRPC]
    private void RPC_Atk(int id)
    {
        PhotonView ph = PhotonView.Find(id);
        if (ph.IsMine)
        {
            PhotonNetwork.Destroy(ph.gameObject);
        }
    }
}
