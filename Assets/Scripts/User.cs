using System;

public class User {
    
    public string userName;
    public bool firstTime;
    public bool isConnected;
    
    public int[] currentCardsDeck;
    public int[] currentDicesDeck;

    public int[] monsterDeck1;
    public int[] monsterDeck2;
    public int[] monsterDeck3;
    public int[] monsterDeck4;
    
    public int[] diceDeck1;
    public int[] diceDeck2;
    public int[] diceDeck3;
    public int[] diceDeck4;

    public string[] cardDeckName;
    public string[] diceDeckName;
    
    public User(string usernameEnv, bool connect, bool firstTimeCheck) {
        userName = usernameEnv;
        isConnected = connect;
        firstTime = firstTimeCheck;
        
        currentCardsDeck = Array.Empty<int>();;
        currentDicesDeck = Array.Empty<int>();;
        
        monsterDeck1 = Array.Empty<int>();
        monsterDeck2 = Array.Empty<int>();
        monsterDeck3 = Array.Empty<int>();
        monsterDeck4 = Array.Empty<int>();
        
        diceDeck1 = Array.Empty<int>();
        diceDeck2 = Array.Empty<int>();
        diceDeck3 = Array.Empty<int>();
        diceDeck4 = Array.Empty<int>();

        cardDeckName = new string[4];
        diceDeckName = new string[4];
    }
}
