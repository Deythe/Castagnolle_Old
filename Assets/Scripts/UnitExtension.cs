using Photon.Pun;
using UnityEngine;

public class UnitExtension : MonoBehaviour
{
    [SerializeField] private Material ownerMonsterColor;
    [SerializeField] private Material ennemiMonsterColor;
    
    [SerializeField] private Material ownerMonsterColorChoosen;
    [SerializeField] private Material ennemiMonsterColorChoosen;
    
    [SerializeField] private GameObject unitParent;
    [SerializeField] private MeshRenderer ms;
    [SerializeField] private Transform center;
    [SerializeField] private PhotonView view;
    
    public void Init(GameObject unit)
    {
        unitParent = unit;
        
        if (view.AmOwner)
        {
            ms.material.mainTexture = ownerMonsterColor.mainTexture;
        }
        else
        {
            ms.material.mainTexture = ennemiMonsterColor.mainTexture;
        }
        
        unitParent.GetComponent<Monster>().GetCenters().Add(center);
        unitParent.GetComponent<Monster>().GetMeshRenderers().Add(ms);
        PlacementManager.instance.AddExtentionMonsterBoard(gameObject,unitParent);
    }

    public void SetParent(GameObject parent)
    {
        unitParent = parent;
    }
    
    public PhotonView GetView()
    {
        return view;
    }
}
