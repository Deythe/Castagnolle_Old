using Photon.Pun;
using UnityEngine;

public class UnitExtension : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] private Material ownerMonsterColor;
    [SerializeField] private Material ennemiMonsterColor;

    [SerializeField] private GameObject unitParent;
    [SerializeField] private MeshRenderer ms;
    [SerializeField] private Transform center;
    [SerializeField] private PhotonView view;
    
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        
        if (view.AmOwner)
        {
            ms.material.mainTexture = ownerMonsterColor.mainTexture;
        }
        else
        {
            ms.material.mainTexture = ennemiMonsterColor.mainTexture;
        }
        
        unitParent = PhotonView.Find((int)instantiationData[0]).gameObject;
        unitParent.GetComponent<Monster>().GetExtention().Add(gameObject);
        unitParent.GetComponent<Monster>().GetCenters().Add(center);
        unitParent.GetComponent<Monster>().GetMeshRenderers().Add(ms);
        PlacementManager.instance.AddExtentionMonsterBoard(gameObject,unitParent);
    }
}
