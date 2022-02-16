using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private int[] monsterDeck = new int[20];
    void Start()
    {
        RandomiseDeck();
    }

    void RandomiseDeck()
    {
        for (int i = 0; i < monsterDeck.Length; i++)
        {
            monsterDeck[i] = Random.Range(1, 7);
        }
    }
}
