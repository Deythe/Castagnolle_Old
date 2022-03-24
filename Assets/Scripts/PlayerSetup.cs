using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public static PlayerSetup instance;
    
    [SerializeField] private Camera cam;
    [SerializeField] private List<Transform> spawnerDice;
    [SerializeField] private List<Transform> spawnerGauge;
    void Start()
    {
        instance = this;
        cam.enabled = true;
        DiceManager.instance.InitDice();
        DiceManager.instance.InitGaugeDice();
    }

    public Camera GetCam()
    {
        return cam;
    }

    public List<Transform> GetSpawner()
    {
        return spawnerDice;
    }
    
    public List<Transform> GetGaugeSpawner()
    {
        return spawnerGauge;
    }

}
