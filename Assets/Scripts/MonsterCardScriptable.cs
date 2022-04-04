
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create New Monster", fileName = "Monster")]
public class MonsterCardScriptable : ScriptableObject
{
    public int atk;
    public List<int> resources;
    public GameObject prefabs;
}
