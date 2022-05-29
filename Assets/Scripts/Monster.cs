using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Monster : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] private GameObject card;
    [SerializeField] private Sprite bigCard;
    
    [SerializeField] private SkinnedMeshRenderer model;
    [SerializeField] private Material modelTexture;

    [SerializeField] private PhotonView view;
    
    [SerializeField] private int owner = 0;
    [SerializeField] private int atk;
    [SerializeField] private int id;

    [SerializeField] private List<Transform> centers;
    [SerializeField] private List<MeshRenderer> mrs;
    
    [SerializeField] private GameObject hpPackage;
    [SerializeField] private TMP_Text hpViewNormal;
    [SerializeField] private MeshRenderer glow;
    
    [SerializeField] private bool attacked;
    [SerializeField] private bool isMovable;
    [SerializeField] private bool isPeon;
    [SerializeField] private bool isChampion;
    [SerializeField] private Animator animator;
    
    private List<IEffects> effects = new List<IEffects>();
    private List<GameObject> extension = new List<GameObject>();
    private RaycastHit hit;

    public GameObject p_model
    {
        get => model.gameObject;
    }

    public GameObject Stats
    {
        get => card;
    }
    public Sprite BigCard
    {
        get => bigCard;
    }
    public int ID
    {
        get => id;
    }

    public int Atk
    {
        get => atk;
        set
        {
            atk = value;
            hpViewNormal.text = ""+atk;
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
    
    public bool p_isPeon
    {
        get => isPeon;
        set
        {
            isPeon = value;
        }
    }

    
    public bool p_isMovable
    {
        get => isMovable;
        set
        {
            isMovable = value;
            Debug.Log(isMovable);
            if (isMovable)
            {
                ChangeMeshRenderer(0);
            }
            else
            {
                ChangeMeshRenderer(4);
            }
        }
    }
    
    public bool p_isChampion
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
            if (UiManager.instance.ViewTacticsOn)
            {
                hpPackage.transform.rotation = Quaternion.Euler(90, hpPackage.transform.rotation.eulerAngles.y,hpPackage.transform.rotation.eulerAngles.z);
            }
            else
            {
                hpPackage.transform.rotation = Quaternion.Euler(45, hpPackage.transform.rotation.eulerAngles.y,hpPackage.transform.rotation.eulerAngles.z);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (model != null)
        {
            model.material = modelTexture;
        }

        AddAllEffects();
        owner = view.OwnerActorNr;

        if (view.AmOwner)
        {
            glow.material = RoundManager.instance.p_listMatPlayerGlow[0];
        }
        else
        {
            glow.material = RoundManager.instance.p_listMatPlayerGlow[1];
        }
        
        if (card!=null)
        {
            Atk = card.GetComponent<CardData>().Atk;
            isChampion = card.GetComponent<CardData>().IsChampion;
            bigCard = card.GetComponent<CardData>().BigCard;
        }
        else
        {
            Atk = atk;
            p_isChampion = isChampion;
        }

        id = view.ViewID;

        InitColorsTiles();
        hpPackage.SetActive(true);
        hpPackage.transform.rotation = Quaternion.Euler(90, PlayerSetup.instance.transform.rotation.eulerAngles.y, 0);
        PlacementManager.instance.AddMonsterBoard(gameObject);
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
        foreach (var effet in effects)
        {
            if (effet.GetPhaseActivation().Equals(i))
            {
                return effet.GetUsed();
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
            if (effet.GetPhaseActivation() == 3 || effet.GetPhaseActivation() == 1 && view.AmOwner)
            {
                effet.SetUsed(false);
                model.gameObject.layer = 7;
            }
        }
    }

    private void AddAllEffects()
    {
        foreach (IEffects effet in gameObject.GetComponents(typeof(IEffects)))
        {
            effects.Add(effet);
            if (view.AmOwner && (effet.GetPhaseActivation() == 3 || effet.GetPhaseActivation() == 1))
            {
                model.gameObject.layer = 7;
            }
        }

    }

    private void OnDestroy()
    {
        if (owner != 0) 
        {
            if (EffectManager.instance != null)
            {
                EffectManager.instance.View.RPC("RPC_PlayAnimation", RpcTarget.AllViaServer, 1, transform.position.x,
                    0.6f,
                    transform.position.z, 3f);
            }
        }

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
        BattlePhaseManager.instance.IsAttacking = false;
    }
    
    public void BeChoosen()
    {
        foreach (MeshRenderer ms in mrs)
        {
            if (view.AmOwner)
            {
                if (!isMovable && Attacked)
                {
                    ms.material = PlacementManager.instance.p_listMaterial[6];
                }
                else
                {
                    ms.material = PlacementManager.instance.p_listMaterial[2];
                }
            }
            else
            {
                if (!isMovable && Attacked)
                {
                    ms.material.color = PlacementManager.instance.p_listMaterial[7].color;
                }
                else
                {
                    ms.material.color = PlacementManager.instance.p_listMaterial[3].color;
                }
            }
        }
    }

    void ChangeMeshRenderer(int index)
    {
        foreach (var ms in mrs)
        {   
            if (view.AmOwner)
            {
                ms.material = PlacementManager.instance.p_listMaterial[index];
            }
            else
            {
                ms.material = PlacementManager.instance.p_listMaterial[index + 1];
            }
        }
    }

    public void InitColorsTiles()
    {
        foreach (var ms in mrs)
        {   
            if (view.AmOwner)
            {
                if (!isMovable)
                {
                    ms.material = PlacementManager.instance.p_listMaterial[4];
                }
                else
                {
                    ms.material = PlacementManager.instance.p_listMaterial[0];
                }
            }
            else
            {
                if (!isMovable)
                {
                    ms.material = PlacementManager.instance.p_listMaterial[5];
                }
                else
                {
                    ms.material = PlacementManager.instance.p_listMaterial[1];
                }
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

    public List<GameObject> GetExtention()
    {
        return extension;
    }
}
