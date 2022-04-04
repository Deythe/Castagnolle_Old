using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User {
    
    public string userName;
    public bool isConnected;
    public int[] currentDeck;
    
    public User() {
    }

    public User(string usernameEnv, bool b, int[] tab) {
        userName = usernameEnv;
        isConnected = b;
        currentDeck = tab;
    }
}
