using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSetup : MonoBehaviour
{
    public static PlayerSetup instance;
    [SerializeField] private Camera cam;
    [SerializeField] private List<Transform> spawnerDice;
    [SerializeField] private List<Transform> spawnerGauge;
    private bool isShaking;
    private Vector3 startCamPos;
    private Quaternion startCamRot;
    private float randomX, randomZ;
    private float shakeAmount = 0.1f;
    void Start()
    {
        instance = this;
        cam.enabled = true;

        DiceManager.instance.InitDiceMesh();
        DiceManager.instance.InitGaugeDiceMesh();
        
        EffectManager.instance.InstantiateAllEffect();
            
        startCamPos = cam.transform.position;
        startCamRot = cam.transform.rotation;
        
        UiManager.instance.CanvasPublic.worldCamera = cam;
        UiManager.instance.Waiting.SetActive(false);
        UiManager.instance.InitPlayerMarkers();
        
        if (RoundManager.instance.p_localPlayerTurn.Equals(2))
        {
            DiceManager.instance.p_diceGauge[0] = DiceListScriptable.enumRessources.Neutral;
            DiceManager.instance.View.RPC("RPC_SynchGaugeDice", RpcTarget.AllViaServer,
                DiceManager.instance.DiceGaugeObjet[0].GetComponent<PhotonView>().ViewID, true, 4);
        }
    }

    private void Update()
    {
        if (isShaking)
        {
            randomX = Random.Range(-1, 1);
            randomZ = Random.Range(-1, 1);
            transform.position = new Vector3(0+randomX*shakeAmount, 0, 0+randomZ*shakeAmount);
        }
    }

    public Vector3 p_startCamPos
    {
        get => startCamPos;
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

    public void ChangeView(bool b)
    {
        if (b)
        {
            cam.transform.DOMove(new Vector3(0, 20, 3f * Math.Sign(startCamPos.z)), 0.3f);
            cam.transform.DORotateQuaternion(Quaternion.Euler(90, startCamRot.eulerAngles.y, startCamRot.eulerAngles.z), 0.3f);
        }
        else
        {
            cam.transform.DOMove(startCamPos, 0.3f);
            cam.transform.DORotateQuaternion(startCamRot, 0.3f);
        }
    }

    public void Shaken(float f)
    {
        isShaking = true;
        StartCoroutine(CoroutineShaken(f));
    }

    IEnumerator CoroutineShaken(float f)
    {
        yield return new WaitForSeconds(f);
        isShaking = false;
        transform.position = Vector3.zero;
    }

}
