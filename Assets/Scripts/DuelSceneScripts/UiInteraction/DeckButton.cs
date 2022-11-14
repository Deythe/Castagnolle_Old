using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckButton : MonoBehaviour
{
    [SerializeField] private Image border;
    [SerializeField] private Sprite unselected;
    [SerializeField] private Sprite selected;
    [SerializeField] private Transform parent;
    [SerializeField] private int[] premakeCardsDeck, premakeDicesDeck;
    private bool select;
    public void Action()
    {
        SoundManager.instance.PlaySFXSound(0, 0.07f);
        
        if (!select)
        {
            MenuManager.instance.PlayButton.interactable = true;
            LocalSaveManager.instance.user.currentCardsDeck = premakeCardsDeck;
            LocalSaveManager.instance.user.currentDicesDeck = premakeDicesDeck;
            
            border.sprite = selected;
            
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).gameObject.transform != transform)
                {
                    parent.GetChild(i).GetComponent<DeckButton>().select = false;
                    parent.GetChild(i).GetComponent<DeckButton>().border.sprite = unselected;
                }
            }
        }
        else
        {
            MenuManager.instance.PlayButton.interactable = false;
            border.sprite = unselected;
        }
        
        select = !select;
    }
}
