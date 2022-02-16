using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public static PlayerSetup instance;
    
    [SerializeField] private Camera cam;

    void Start()
    {
        instance = this;
        cam.enabled = true;
    }

    public Camera GetCam()
    {
        return cam;
    }
    
}
