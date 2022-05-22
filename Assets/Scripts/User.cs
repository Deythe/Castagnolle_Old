using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User {
    
    public string userName;
    public bool isConnected;
    public int[] currentDeck;
    public int[] currentDiceDeck;

    public User() {
    }

    public User(string usernameEnv, bool b) {
        userName = usernameEnv;
        isConnected = b;
        currentDeck = new int[8];
        currentDiceDeck = new int[12];
    }
}
