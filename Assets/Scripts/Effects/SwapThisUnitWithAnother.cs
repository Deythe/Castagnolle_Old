using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class SwapThisUnitWithAnother : MonoBehaviour, IEffects
{
    [SerializeField] private PhotonView view;
    [SerializeField] private List<EffectManager.enumEffectConditionActivation> conditions;
    [SerializeField] private List<EffectManager.enumActionEffect> actions;
    [SerializeField] private bool isEffectAuto;
    [SerializeField] private bool used;
    [SerializeField] private bool isActivable;
    [SerializeField] private EffectManager.enumOrderPriority orderPriority;
    [SerializeField] private MonstreData targetUnit;
    private List<Vector3> positionTilesAlly, positionTilesEnnemy;
    private BoxCollider pivotBoxCollider;
    private List<GameObject> tilesPivot;
    private Vector3 positionPivot;
    private int i;
    
    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                view.RPC("RPC_OnCast", RpcTarget.All,EffectManager.instance.p_unitTarget1.GetComponent<MonstreData>().p_id);
            }
        }
    }

    [PunRPC]
    private void RPC_OnCast(int idUnit)
    {
        targetUnit = PlacementManager.instance.FindMobWithID(idUnit);
        PlacementManager.instance.RemoveMonsterBoard(GetComponent<MonstreData>().p_id);
        PlacementManager.instance.RemoveMonsterBoard(idUnit);
        
        tilesPivot = new List<GameObject>();
        positionTilesAlly =  new List<Vector3>();
        positionTilesEnnemy =  new List<Vector3>();

        for (i = 0; i < targetUnit.transform.childCount; i++)
        {
            if (targetUnit.transform.GetChild(i).transform.CompareTag("Tile"))
            {
                positionTilesEnnemy.Add(targetUnit.transform.GetChild(i).transform.position);
            }
        }

        for (i = 0; i < transform.childCount; i++)
        {
            if (targetUnit.transform.GetChild(i).transform.CompareTag("Tile"))
            {
                tilesPivot.Add(targetUnit.transform.GetChild(i).gameObject);
            }
        }

        positionPivot = targetUnit.transform.position;
        targetUnit.transform.position = transform.position;
        transform.position = positionPivot;
        
        for (i = 0; i < targetUnit.transform.childCount; i++)
        {
            if (targetUnit.transform.GetChild(i).transform.CompareTag("Tile"))
            {
                tilesPivot.Add(targetUnit.transform.GetChild(i).gameObject);
            }
        }

        for (i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).transform.CompareTag("Tile"))
            {
                transform.GetChild(i).SetParent(targetUnit.transform);
            }
        }
        
        for (i = 0; i < tilesPivot.Count; i++)
        {
            tilesPivot[i].transform.SetParent(transform);
        }
        
        /*
        for (i = 0; i < targetUnit.transform.childCount; i++)
        {
            if (targetUnit.transform.GetChild(i).transform.CompareTag("Tile"))
            {
                targetUnit.transform.GetChild(i).transform.position = positionTilesAlly[i];
            }
        }
        
        for (i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).transform.CompareTag("Tile"))
            {
                transform.GetChild(i).transform.position = positionTilesEnnemy[i];
            }
        }*/
        
        EffectManager.instance.CancelSelection();
    }

    /*
    [PunRPC]
    private void RPC_OnCast(int idUnit)
    {
        targetUnit = PlacementManager.instance.FindMobWithID(idUnit);
        PlacementManager.instance.RemoveMonsterBoard(GetComponent<MonstreData>().p_id);
        PlacementManager.instance.RemoveMonsterBoard(idUnit);
        
        tilesPivot = new List<GameObject>();
        boxCollidersCenterPivot = new List<Vector3>();
        boxCollidersSizePivot = new List<Vector3>();
        positionPivot = targetUnit.p_model.transform.position;
        targetUnit.p_model.transform.position = transform.position;
        GetComponent<MonstreData>().p_model.transform.position = positionPivot;

        for (i = 0; i < targetUnit.transform.childCount; i++)
        {
            if (targetUnit.transform.GetChild(i).transform.CompareTag("Tile"))
            {
                tilesPivot.Add(targetUnit.transform.GetChild(i).gameObject);
            }
        }

        for (i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).transform.CompareTag("Tile"))
            {
                transform.GetChild(i).SetParent(targetUnit.transform);
            }
        }
        
        for (i = 0; i < tilesPivot.Count; i++)
        {
            tilesPivot[i].transform.SetParent(transform);
        }
        
        PlacementManager.instance.AddMonsterBoard(targetUnit.gameObject);
        PlacementManager.instance.AddMonsterBoard(gameObject);

        targetUnit.p_hpPackage.transform.position = new Vector3(targetUnit.p_model.transform.position.x, targetUnit.p_hpPackage.transform.position.y, targetUnit.p_model.transform.position.z);
        GetComponent<MonstreData>().p_hpPackage.transform.position = new Vector3(GetComponent<MonstreData>().p_model.transform.position.x,GetComponent<MonstreData>().p_hpPackage.transform.position.y, GetComponent<MonstreData>().p_model.transform.position.z);
        
        
        for (i = targetUnit.GetComponents<BoxCollider>().Length - 1; i >= 0; i--)
        {
            boxCollidersCenterPivot.Add(targetUnit.GetComponents<BoxCollider>()[i].center);
            boxCollidersSizePivot.Add(targetUnit.GetComponents<BoxCollider>()[i].size);
            Destroy(targetUnit.GetComponents<BoxCollider>()[i]);
        }
        
        for (i = GetComponents<BoxCollider>().Length - 1; i >= 0; i--)
        {
            pivotBoxCollider = targetUnit.gameObject.AddComponent<BoxCollider>();
            pivotBoxCollider.center = GetComponents<BoxCollider>()[i].center;
            pivotBoxCollider.size = GetComponents<BoxCollider>()[i].size;
            Destroy(GetComponents<BoxCollider>()[i]);
        }
        
        /*
        
        for (int j = 0; j < boxCollidersCenterPivot.Count; j++)
        {
            pivotBoxCollider = gameObject.AddComponent<BoxCollider>();
            pivotBoxCollider.center = boxCollidersCenterPivot[j];
            pivotBoxCollider.size = boxCollidersSizePivot[j];
        }
        
        /*
        for (i = GetComponents<BoxCollider>().Length - 1; i >= 0; i--)
        {
            Destroy(GetComponents<BoxCollider>()[i]);
        }

        for (int j = 0; j < boxCollidersCenterPivot.Count; j++)
        {
            pivotBoxCollider = gameObject.AddComponent<BoxCollider>();
            pivotBoxCollider.center = boxCollidersCenterPivot[j];
            pivotBoxCollider.size = boxCollidersSizePivot[j];
        }


        GetComponent<MonstreData>().p_model.layer = 6;
        EffectManager.instance.CancelSelection();
        
        
    }
        */
    public void TransferEffect(IEffects effectMother)
    {
        view = gameObject.GetPhotonView();
        conditions = new List<EffectManager.enumEffectConditionActivation>(effectMother.GetConditions());
        actions = new List<EffectManager.enumActionEffect>(effectMother.GetActions());
        used = effectMother.GetUsed();
        isActivable = effectMother.GetIsActivable();
    }
    
    public PhotonView GetView()
    {
        return view;
    }
    
    public List<EffectManager.enumEffectConditionActivation> GetConditions()
    {
        return conditions;
    }
    
    public List<EffectManager.enumActionEffect> GetActions()
    {
        return actions;
    }
    
    public bool GetIsActivable()
    {
        return isActivable;
    }

    public void SetIsActivable(bool b)
    {
        isActivable = b;
    }
    
    public EffectManager.enumOrderPriority GetOrderPriority()
    {
        return orderPriority;
    }

    public bool GetUsed()
    {
        return used;
    }

    public void SetUsed(bool b)
    {
        used = b;
    }
    
    public void ResetEffect()
    {
        used = false;
    }

    public bool GetIsEffectAuto()
    {
        return isEffectAuto;
    }

    public void SetIsEffectAuto(bool b)
    {
        isEffectAuto = b;
    }
    
    public void CancelEffect()
    {
        
    }
}
