using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Monster : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{    
    [SerializeField] private Material ownerMonsterColor;
    [SerializeField] private Material ennemiMonsterColor;
    
    [SerializeField] private Material ownerMonsterColorChoosen;
    [SerializeField] private Material ennemiMonsterColorChoosen;
    
    [SerializeField] private PhotonView view;
    
    [SerializeField] private int owner = 0;
    [SerializeField] private int atk;
    [SerializeField] private int id;

    [SerializeField] private List<Transform> centers;
    [SerializeField] private List<MeshRenderer> mrs;
    
    [SerializeField] private TMP_Text hpView;
    [SerializeField] private Canvas canvasRenderer;
    
    [SerializeField] private bool attacked;
    [SerializeField] private int status; //0 = normal, 1 = Immobile
    [SerializeField] private Animator animator;
    [SerializeField] private bool isChampion;
    private List<IEffects> effects = new List<IEffects>();
    private List<GameObject> extension = new List<GameObject>();
    private RaycastHit hit;

    public int ID
    {
        get => id;
    }
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
            hpView.text = ""+atk;
            CheckDeath();
        }
    }

    public void CheckDeath()
    {
        if (atk <= 0)
        {
            if (view.AmOwner)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public Animator Animator
    {
        get => animator;
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


    private void Update()
    {
        if (owner!=0)
        {
            hpView.enabled = UiManager.instance.ViewTacticsOn;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        
        AddAllEffects();
        owner = view.OwnerActorNr;
        
        if (instantiationData!=null)
        {
            Atk = (int) instantiationData[0];
            isChampion = (bool) instantiationData[1];
        }

        id = view.ViewID;

        foreach (var ms in mrs)
        {   
            if (view.AmOwner)
            {
                ms.material.color = ownerMonsterColor.color;
            }
            else
            {
                ms.material.color = ennemiMonsterColor.color;
            }
        }

        canvasRenderer.worldCamera = PlayerSetup.instance.GetCam();
        canvasRenderer.GetComponentInParent<RectTransform>().rotation = Quaternion.Euler(90, PlayerSetup.instance.transform.rotation.eulerAngles.y, 0);
        
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

    public bool ReturnUsedOfAnEffect(int i)
    {
        return effects[i].GetUsed();
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
            if (effet.GetPhaseActivation() == 3 || effet.GetPhaseActivation() == 1 && view.AmOwner)
            {
                effet.SetUsed(false);
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
        foreach (IEffects effet in effects)
        {
            effet.OnCast(2);
        }
        
        for (int i = extension.Count - 1; i >= 0; i--)
        {
            if (view.AmOwner)
            {
                PhotonNetwork.Destroy(extension[i]);
            }
        }
        
        PlacementManager.instance.RemoveMonsterBoard(id);
    }
    public void BeChoosen()
    {
        foreach (MeshRenderer ms in mrs)
        {
            if (view.AmOwner)
            {
                ms.material.color = ownerMonsterColorChoosen.color;
            }
            else
            {
                ms.material.color = ennemiMonsterColorChoosen.color;
            }
        }
    }

    public void NotChossen()
    {
        foreach (var ms in mrs)
        {
            if (view.AmOwner)
            {
                ms.material.color = ownerMonsterColor.color;
            }
            else
            {
                ms.material.color = ennemiMonsterColor.color;
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
