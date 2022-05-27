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
    
    [SerializeField] private TMP_Text hpView;

    [SerializeField] private bool attacked;
    [SerializeField] private bool isMovable;
    [SerializeField] private bool isPeon;
    [SerializeField] private bool isChampion;
    [SerializeField] private Animator animator;
    
    private List<IEffects> effects = new List<IEffects>();
    private List<GameObject> extension = new List<GameObject>();
    private RaycastHit hit;

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
            hpView.enabled = UiManager.instance.ViewTacticsOn;
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
        hpView.GetComponentInParent<RectTransform>().rotation = Quaternion.Euler(90, PlayerSetup.instance.transform.rotation.eulerAngles.y, 0);
        
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
        if (owner != 0) 
        {
            if (EffectManager.instance != null && gameObject.activeSelf)
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
                    ms.material.color = PlacementManager.instance.p_listMaterial[6].color;
                }
                else
                {
                    ms.material.color = PlacementManager.instance.p_listMaterial[2].color;
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

    public void InitColorsTiles()
    {
        foreach (var ms in mrs)
        {   
            if (view.AmOwner)
            {
                if (!isMovable)
                {
                    ms.material.color = PlacementManager.instance.p_listMaterial[4].color;
                }
                else
                {
                    ms.material.color = PlacementManager.instance.p_listMaterial[0].color;
                }
            }
            else
            {
                if (!isMovable)
                {
                    ms.material.color = PlacementManager.instance.p_listMaterial[5].color;
                }
                else
                {
                    ms.material.color = PlacementManager.instance.p_listMaterial[1].color;
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
