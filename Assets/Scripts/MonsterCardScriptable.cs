
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create New Monster", fileName = "Monster")]
public class MonsterCardScriptable : ScriptableObject
{
    public string name;
    public int atk;
    public List<int> resources;
}
