using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private DeckScriptable deck;
    [SerializeField] private DiceDeckScriptable diceDeck;
    public void Action()
    {
        FireBaseManager.instance.User.currentDeck = deck.indexCrea;
        FireBaseManager.instance.User.currentDiceDeck = diceDeck.diceDeck;
        MenuManager.instance.PlayButton.interactable = true;
    }
}
