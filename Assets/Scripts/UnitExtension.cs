using Photon.Pun;
using UnityEngine;

public class UnitExtension : MonoBehaviour
{
    [SerializeField] private GameObject unitParent;
    [SerializeField] private MeshRenderer ms;
    [SerializeField] private Transform center;
    [SerializeField] private PhotonView view;

    public GameObject p_unitParent
    {
        get => unitParent;
    }
    public void Init(int idmore)
    {
        unitParent = PlacementManager.instance.SearchMobWithID(idmore).gameObject;
        
        if (view.AmOwner)
        {
            if (unitParent.GetComponent<MonstreData>().p_isMovable)
            {
                ms.material = PlacementManager.instance.p_listMaterial[0];
            }
            else
            {
                ms.material = PlacementManager.instance.p_listMaterial[4];
            }
        }
        else
        {
            if (unitParent.GetComponent<MonstreData>().p_isMovable)
            {
                ms.material = PlacementManager.instance.p_listMaterial[1];
            }
            else
            {
                ms.material = PlacementManager.instance.p_listMaterial[5];
            }
        }
        
        unitParent.GetComponent<MonstreData>().GetExtention().Add(gameObject);
        unitParent.GetComponent<MonstreData>().GetCenters().Add(center);
        unitParent.GetComponent<MonstreData>().GetMeshRenderers().Add(ms);
        PlacementManager.instance.AddExtentionMonsterBoard(gameObject,unitParent);
    }
}
