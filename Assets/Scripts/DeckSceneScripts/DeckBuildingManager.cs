using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckBuildingManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] parchment;
    [SerializeField] private RectTransform pages;
    [SerializeField] private CanvasScaler canvas;
    private int currentPages;
    private void Awake()
    {
        PhotonNetwork.Disconnect();
    }

    private void Start()
    {
        currentPages = 1;
        foreach (RectTransform parche in parchment)
        {
            parche.DOLocalMoveX(80, 0.8f).SetEase(Ease.OutBack).OnComplete(()=>parche.GetComponent<Button>().interactable = true);
        }
    }

    public void GoToPage2()
    {
        pages.DOLocalMoveX(-canvas.referenceResolution.x, 0.5f).SetEase(Ease.Linear);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
