using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Monster : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
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
    
    [SerializeField] private CardData card;

    [SerializeField] private bool attacked; 
    
    private MonsterCardScriptable stats;

    private List<IEffects> effects = new List<IEffects>();
    private List<GameObject> extension;
    private RaycastHit hit;

    private void Awake()
    {
        stats = card.GetStat();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        AddAllEffects();
        owner = view.OwnerActorNr;
        atk = stats.atk;
        id = view.ViewID;
        extension = new List<GameObject>();
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

    public void ActivateEffects(int i)
    {
        foreach (var effet in effects)
        {
            if (effet.GetPhaseActivation().Equals(i))
            {
                effet.OnCast();
            }
        }
    }

    private void AddAllEffects()
    {
        foreach (IEffects effet in gameObject.GetComponents(typeof(IEffects)))
        {
            effects.Add(effet);
        }    
    }
    
    private void OnDestroy()
    {
        for (int i = 0; i < PlacementManager.instance.GetBoard().Count; i++)
        {
            if (PlacementManager.instance.GetBoard()[i].monster.GetComponent<Monster>().id == id)
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

    public MonsterCardScriptable GetStat()
    {
        return stats;
    }
    
    public List<Transform> GetCenters()
    {
        return centers;
    }
    
    public List<MeshRenderer> GetMeshRenderers()
    {
        return mrs;
    }

    public void SetAtk(int i)
    {
        atk = i;
    }
    
    public int GetOwner()
    {
        return owner;
    }

    public int GetAtk()
    {
        return atk;
    }

    public PhotonView GetView()
    {
        return view;
    }

    public List<GameObject> GetExtention()
    {
        return extension;
    }
    
    public bool GetAttacked()
    {
        return attacked;
    }
    
    public void SetAttacked(bool b)
    {
        attacked = b;
    }
}
