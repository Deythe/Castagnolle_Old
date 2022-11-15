using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text numberCard;
    [SerializeField] private TMP_Text deckName;
    private int[] _deck;
    private int counter=0;
    public int[] deck
    {
        get => _deck;
        set
        {
            _deck = value;
        }
    }

    public void UpdateDeckName(string newName)
    {
        deckName.text = ""+newName;
    }

    public void UpdateCounterObject()
    {
        counter = 0;
        for (int i = 0; i < _deck.Length; i++)
        {
            if (_deck[i] != -1)
            {
                counter++;
            }
        }

        numberCard.text = counter + "/" +_deck.Length;;
    }
}
