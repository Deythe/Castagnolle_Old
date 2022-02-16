using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlatformMonster : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{    
    [SerializeField] private Material ownerMonsterColor;
    [SerializeField] private Material ennemiMonsterColor;
    
    [SerializeField] private Material ownerMonsterColorChoosen;
    [SerializeField] private Material ennemiMonsterColorChoosen;
    
    [SerializeField] private PhotonView view;
    
    [SerializeField] private int owner;
    [SerializeField] private int atk;
    [SerializeField] private int id;

    [SerializeField] private List<Transform> centers;
    [SerializeField] private List<MeshRenderer> mrs;
    
    [SerializeField] private MonsterCardScriptable stats;
    
    private RaycastHit hit;
    
    
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        owner = view.OwnerActorNr;
        atk = stats.atk;
        id = view.ViewID;

        foreach (var ms in mrs)
        {   
            if (view.AmOwner)
            {
                ms.material.mainTexture = ownerMonsterColor.mainTexture;
            }
            else
            {
                ms.material.mainTexture = ennemiMonsterColor.mainTexture;
            }
        }
    
        PlacementManager.instance.AddMonsterBoard(gameObject);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < PlacementManager.instance.GetBoard().Count; i++)
        {
            if (PlacementManager.instance.GetBoard()[i].monster.GetComponent<PlatformMonster>().id == id)
            {
                PlacementManager.instance.GetBoard().Remove(PlacementManager.instance.GetBoard()[i]);
            }
        }
    }

    public void BeChoosen()
    {
        foreach (var ms in mrs)
        {
            if (view.AmOwner)
            {
                ms.material.mainTexture = ownerMonsterColorChoosen.mainTexture;
            }
            else
            {
                ms.material.mainTexture = ennemiMonsterColorChoosen.mainTexture;
            }
        }
    }
    
    public void NotChossen()
    {
        foreach (var ms in mrs)
        {
            if (view.AmOwner)
            {
                ms.material.mainTexture = ownerMonsterColor.mainTexture;
            }
            else
            {
                ms.material.mainTexture = ennemiMonsterColor.mainTexture;
            }
        }
    }

    public List<Transform> GetCenters()
    {
        return centers;
    }

    public void SetStats(MonsterCardScriptable monster)
    {
        stats = monster;
    }

    public int GetOwner()
    {
        return owner;
    }
    
    public void SetOwner(int i)
    {
        owner = i;
    }
    
    public void SetAtk(int a)
    {
        atk = a;
    }
    
    public int GetAtk()
    {
        return atk;
    }
}
