using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutoManager : MonoBehaviour
{
    [SerializeField] private Button nextArrow;
    [SerializeField] private GameObject beforeArrow;

    [SerializeField] private Image currentPage;
    [SerializeField] private List<Sprite> listTutoPage;
    [SerializeField] private List<VideoClip> listVideoClip;
    [SerializeField] private VideoPlayer player1;
    [SerializeField] private Image gradient;

    private int currentIndexPage = 0;
    private int maxPageCount = 10;


    void Start()
    {
        PhotonNetwork.Disconnect();
        player1.Prepare();
        
        FireBaseManager.instance.User.firstTime = false;
        if (FireBaseManager.instance.User.isConnected)
        {
            FireBaseManager.instance.FirstTimeChecked();
        }
    }

    public void ChangePage(bool b)
    {
        gradient.DOFade(1, 0.2f).OnComplete(() => CheckVideoOrSprite());
        
        if (b)
        {
            SoundManager.instance.PlaySFXSound(0, 0.07f);
            currentIndexPage++;
            
        }
        else
        {
            SoundManager.instance.PlaySFXSound(1, 0.07f);
            currentIndexPage--;
        }

        if (currentIndexPage == 0)
        {
            beforeArrow.SetActive(false);
        }
        else
        {
            beforeArrow.SetActive(true);
        }
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    void CheckVideoOrSprite()
    {
        gradient.DOFade(0, 0.2f);
        player1.Stop();
        switch (currentIndexPage)
        {
            case 0 :
                currentPage.enabled = true;
                currentPage.sprite = listTutoPage[0];
                break;
            case 1 :
                currentPage.enabled = false;
                player1.clip = listVideoClip[0];
                player1.Prepare();
                player1.Play();
                break;
            case 2:
                currentPage.enabled = true;
                currentPage.sprite = listTutoPage[1];
                break;
            case 3 :
                currentPage.enabled = false;
                player1.clip = listVideoClip[1];
                player1.Prepare();
                player1.Play();
                break;
            case 4:
                currentPage.enabled = true;
                currentPage.sprite = listTutoPage[2];
                break;
            case 5:
                currentPage.enabled = false;
                player1.clip = listVideoClip[2];
                player1.Prepare();
                player1.Play();
                break;
            case 6:
                currentPage.enabled = false;
                player1.clip = listVideoClip[3];
                player1.Prepare();
                player1.Play();
                break;
            case 7:
                currentPage.enabled = false;
                player1.clip = listVideoClip[4];
                player1.Prepare();
                player1.Play();
                break;
            case 8:
                currentPage.enabled = false;
                player1.clip = listVideoClip[5];
                player1.Prepare();
                player1.Play();
                break;
            case 9:
                currentPage.enabled = false;
                player1.clip = listVideoClip[6];
                player1.Prepare();
                player1.Play();
                break;
            case 10 :
                currentPage.enabled = true;
                currentPage.sprite = listTutoPage[3];
                break;
            case 11:
                GoToMainMenu();
                break;
        }
    }
}