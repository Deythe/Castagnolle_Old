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
    private int atk;
    [SerializeField] private int id;

    [SerializeField] private List<Transform> centers;
    [SerializeField] private List<MeshRenderer> mrs;


    [SerializeField] private bool attacked;
    [SerializeField] private int status; //0 = normal, 1 = Immobile
    
    private List<IEffects> effects = new List<IEffects>();
    private List<GameObject> extension = new List<GameObject>();
    private RaycastHit hit;
    private bool isChampion;

    public int Status
    {
        get => status;
        set
        {
            status = value;
        }
    }
    public int Atk
    {
        get => atk;
        set
        {
            atk = value;
        }
    }
    public bool IsChampion
    {
        get => isChampion;
        set
        {
            isChampion = value;
        }
    }
    
    public bool Attacked
    {
        get => attacked;
        set
        {
            attacked = value;
        }
    }
    

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        
        AddAllEffects();
        owner = view.OwnerActorNr;
        
        if (instantiationData!=null)
        {
            atk = (int) instantiationData[0];
            isChampion = (bool) instantiationData[1];
        }

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
        ActivateEffects(0);
    }

    public bool HaveAnEffectThisTurn(int i)
    {
        foreach (var effet in effects)
        {
            if (effet.GetPhaseActivation().Equals(i) && !effet.GetUsed())
            {
                return true;
            }
        }

        return false;
    }

    public void ActivateEffects(int i)
    {
        foreach (var effet in effects)
        {
            if (!effet.GetUsed())
            {
                effet.OnCast(i);
            }
        }
    }

    public void ReActivadeAllEffect()
    {
        foreach (var effet in effects)
        {
            if (effet.GetPhaseActivation() == 3)
            {
                effet.SetUsed(true);
            }
        }
    }

    private void AddAllEffects()
    {
        if (view.AmOwner)
        {
            foreach (IEffects effet in gameObject.GetComponents(typeof(IEffects)))
            {
                effects.Add(effet);
            }
        }
    }
    
    private void OnDestroy()
    {
        foreach (IEffects effet in effects)
        {
            effet.OnCast(2);
        }
        
        for (int i = extension.Count - 1; i >= 0; i--)
        {
            PhotonNetwork.Destroy(extension[i]);
        }

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
        foreach (MeshRenderer ms in mrs)
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
    
    public List<MeshRenderer> GetMeshRenderers()
    {
        return mrs;
    }
    public int GetOwner()
    {
        return owner;
    }

    public PhotonView GetView()
    {
        return view;
    }

    public List<GameObject> GetExtention()
    {
        return extension;
    }
}
