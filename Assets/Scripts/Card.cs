using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private GameObject go;
    public void ChangePhase()
    {
        RoundManager.instance.SetStateRound(2);
        PlacementManager.instance.SetGOPrefabsMonster(go);
    }
}
