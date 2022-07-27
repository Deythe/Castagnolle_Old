using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User {
    
    public string userName;
    public bool firstTime;
    public bool isConnected;
    public int[] currentDeck;
    public int[] currentDiceDeck;

    public User() {
    }

    public User(string usernameEnv, bool connect, bool firstTimeCheck) {
        userName = usernameEnv;
        isConnected = connect;
        firstTime = firstTimeCheck;
        currentDeck = new int[8];
        currentDiceDeck = new int[6];
    }
}
