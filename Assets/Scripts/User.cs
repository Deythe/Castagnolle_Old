public class User {
    
    public string userName;
    public bool firstTime;
    public bool isConnected;
    public int[] currentDeck;
    public int[] currentDiceDeck;
    
    public int[] monsterDeck1;
    public int[] monsterDeck2;
    public int[] monsterDeck3;
    public int[] monsterDeck4;
    
    public int[] diceDeck1;
    public int[] diceDeck2;
    public int[] diceDeck3;
    public int[] diceDeck4;
    
    public User(string usernameEnv, bool connect, bool firstTimeCheck) {
        userName = usernameEnv;
        isConnected = connect;
        firstTime = firstTimeCheck;
        
        /*currentDeck = new int[8];
        currentDiceDeck = new int[6];
        
        monsterDeck1 = new int[8];
        monsterDeck2 = new int[8];
        monsterDeck3 = new int[8];
        monsterDeck4 = new int[8];
        
        diceDeck1 = new int[9];
        diceDeck2 = new int[9];
        diceDeck3 = new int[9];
        diceDeck4 = new int[9];

        for (int i = 0; i < 8; i++)
        {
            monsterDeck1[i] = -1;
            monsterDeck2[i] = -1;
            monsterDeck3[i] = -1;
            monsterDeck4[i] = -1;

            diceDeck1[i] = -1;
            diceDeck2[i] = -1;
            diceDeck3[i] = -1;
            diceDeck4[i] = -1;
        }
        
        diceDeck1[8] = -1;
        diceDeck2[8] = -1;
        diceDeck3[8] = -1;
        diceDeck4[8] = -1;*/
    }
}
