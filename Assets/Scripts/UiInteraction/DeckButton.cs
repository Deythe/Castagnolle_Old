using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private DeckScriptable deck;
    public void Action()
    {
        FireBaseManager.instance.User.currentDeck = deck.indexCrea;
        FireBaseManager.instance.User.currentDiceDeck = deck.diceDeck.diceDeck;
        MenuManager.instance.PlayButton.interactable = true;
    }
}
