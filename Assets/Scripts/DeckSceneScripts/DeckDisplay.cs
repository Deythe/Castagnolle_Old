using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text numberCard;
    private int[] _deck;
    private int counter=0;
    public int[] deck
    {
        get => _deck;
        set
        {
            _deck = value;
            counter = 0;
            for (int i = 0; i < _deck.Length; i++)
            {
                if (_deck[i] != -1)
                {
                    counter++;
                }
            }

            numberCard.text = counter + "/8";
        }
    }
}
