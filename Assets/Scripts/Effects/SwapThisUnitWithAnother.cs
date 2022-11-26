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

    private List<Vector3> positionTilesUnit, positionTilesTarget, boxCollidersCenterPivot, boxCollidersSizePivot;
    private BoxCollider pivotBoxCollider;
    private List<GameObject> tilesPivot;
    private Vector3 positionPivot, positionModelTarget, positionModelUnit;
    private int i;
    
    public void OnCast(EffectManager.enumEffectConditionActivation condition)
    {
        if (view.AmOwner)
        {
            if (conditions.Contains(condition))
            {
                view.RPC("RPC_OnCast", RpcTarget.All,EffectManager.instance.p_unitTarget1.GetComponent<MonstreData>().p_id);
                GetComponent<MonstreData>().p_model.layer = 6;
                EffectManager.instance.CancelSelection();
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
        positionTilesUnit =  new List<Vector3>();
        positionTilesTarget =  new List<Vector3>();
        boxCollidersCenterPivot = new List<Vector3>();
        boxCollidersSizePivot = new List<Vector3>();

        for (i = 0; i < targetUnit.transform.childCount; i++)
        {
            if (targetUnit.transform.GetChild(i).transform.CompareTag("Tile"))
            {
                positionTilesTarget.Add(targetUnit.transform.GetChild(i).position);
            }
        }
        
        for (i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).transform.CompareTag("Tile"))
            {
                positionTilesUnit.Add(transform.GetChild(i).position);
            }
        }

        positionModelTarget = targetUnit.p_model.transform.position;
        positionModelUnit = GetComponent<MonstreData>().p_model.transform.position;
        
        positionPivot = targetUnit.transform.position;
        targetUnit.transform.position = transform.position;
        transform.position = positionPivot;
        
        targetUnit.p_model.transform.position = positionModelUnit;
        GetComponent<MonstreData>().p_model.transform.position = positionModelTarget;
        
        
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
                transform.GetChild(i).position = positionTilesUnit[i];
                transform.GetChild(i).SetParent(targetUnit.transform);
            }
        }
        
        for (i = 0; i < tilesPivot.Count; i++)
        {
            tilesPivot[i].transform.position = positionTilesTarget[i];
            tilesPivot[i].transform.SetParent(transform);
        }
        
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
        
        for (int j = 0; j < boxCollidersCenterPivot.Count; j++)
        {
            pivotBoxCollider = gameObject.AddComponent<BoxCollider>();
            pivotBoxCollider.center = boxCollidersCenterPivot[j];
            pivotBoxCollider.size = boxCollidersSizePivot[j];
        }
        
        for (int j = 0; j < boxCollidersCenterPivot.Count; j++)
        {
            pivotBoxCollider = gameObject.AddComponent<BoxCollider>();
            pivotBoxCollider.center = boxCollidersCenterPivot[j];
            pivotBoxCollider.size = boxCollidersSizePivot[j];
        }
        
        
        PlacementManager.instance.AddMonsterBoard(targetUnit.gameObject);
        PlacementManager.instance.AddMonsterBoard(gameObject);
    }

  
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
