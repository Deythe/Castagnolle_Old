using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStruct
{
    public string id;
    public float lvl;
    public string meshRendererColor;

    public void InitPlayer(string s)
    {
        id = s;
        lvl = 0;
        meshRendererColor = "black";
    }
}
