using Photon.Pun;
using UnityEngine;

public class UnitExtension : MonoBehaviour
{
    [SerializeField] private Material ownerMonsterColor;
    [SerializeField] private Material ennemiMonsterColor;

    [SerializeField] private GameObject unitParent;
    [SerializeField] private MeshRenderer ms;
    [SerializeField] private Transform center;
    [SerializeField] private PhotonView view;
    
    /*public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

        if (view.AmOwner)
        {
            ms.material.color = ownerMonsterColor.color;
        }
        else
        {
            ms.material.color = ennemiMonsterColor.color;
        }
        
        unitParent = PhotonView.Find((int)instantiationData[0]).gameObject;
        unitParent.GetComponent<Monster>().GetExtention().Add(gameObject);
        unitParent.GetComponent<Monster>().GetCenters().Add(center);
        unitParent.GetComponent<Monster>().GetMeshRenderers().Add(ms);
        PlacementManager.instance.AddExtentionMonsterBoard(gameObject,unitParent);
    }*/

    public void Init(int idmore)
    {
        if (view.AmOwner)
        {
            ms.material.color = ownerMonsterColor.color;
        }
        else
        {
            ms.material.color = ennemiMonsterColor.color;
        }
        
        unitParent = PlacementManager.instance.SearchMobWithID(idmore).gameObject;
        unitParent.GetComponent<Monster>().GetExtention().Add(gameObject);
        unitParent.GetComponent<Monster>().GetCenters().Add(center);
        unitParent.GetComponent<Monster>().GetMeshRenderers().Add(ms);
        PlacementManager.instance.AddExtentionMonsterBoard(gameObject,unitParent);
    }
}
