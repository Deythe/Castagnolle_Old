using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutoManager : MonoBehaviour
{
    [SerializeField] private GameObject nextArrow;
    [SerializeField] private GameObject beforeArrow;

    [SerializeField] private Image currentPage;
    [SerializeField] private List<Sprite> listTutoPage;
    [SerializeField] private List<VideoClip> listVideoClip;
    [SerializeField] private VideoPlayer player1;
    [SerializeField] private Image gradient;

    private int currentIndexPage = 0;
    private int maxPageCount = 9;


    void Start()
    {
        player1.Prepare();
        FireBaseManager.instance.User.firstTime = false;
        if (FireBaseManager.instance.User.isConnected)
        {
            FireBaseManager.instance.FirstTimeChecked();
        }
    }

    public void ChangePage(bool b)
    {
        if (b)
        {
            currentIndexPage++;
        }
        else
        {
            currentIndexPage--;
        }

        if (currentIndexPage == maxPageCount)
        {
            nextArrow.SetActive(false);
        }
        else
        {
            nextArrow.SetActive(true);
        }

        if (currentIndexPage == 0)
        {
            beforeArrow.SetActive(false);
        }
        else
        {
            beforeArrow.SetActive(true);
        }


        if (currentIndexPage < 4)
        {
            player1.Stop();
            currentPage.enabled = true;
            gradient.DOFade(1, 0.2f).OnComplete(ChangePage);
        }
        else
        {
            currentPage.enabled = false;
            player1.Stop();
            player1.clip = listVideoClip[currentIndexPage - 4];
            player1.Prepare();
            gradient.DOFade(1, 0.2f).OnComplete(ChangeVideo);
        }
    }

    void ChangePage()
    {
        currentPage.sprite = listTutoPage[currentIndexPage];
        gradient.DOFade(0, 0.2f);
    }

    void ChangeVideo()
    {
        player1.Play();
        gradient.DOFade(0, 0.2f);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}