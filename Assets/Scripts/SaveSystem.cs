using System.IO;
using UnityEngine;

public static class SaveSystem
{

    public static void SaveNewPlayer(string id)
    {
        PlayerStruct playerStruct = new PlayerStruct();
        playerStruct.InitPlayer(id);
        
        string json = JsonUtility.ToJson(playerStruct);
        File.WriteAllText(Application.dataPath+"/"+id+".txt", json);
    }

    public static PlayerStruct LoadPlayer(string id)
    {
        PlayerStruct playerStruct =
            JsonUtility.FromJson<PlayerStruct>(File.ReadAllText(Application.dataPath + "/" + id +
                                                          ".txt"));
        return playerStruct;
    }

    public static void SaveChangement(string id, int i, string s)
    {
        PlayerStruct playerStruct =
            JsonUtility.FromJson<PlayerStruct>(File.ReadAllText(Application.dataPath + "/" + id +
                                                          ".txt"));
        switch (i)
        {
            case 1:
                playerStruct.meshRendererColor = s;
                break;
        }
        
        string json = JsonUtility.ToJson(playerStruct);
        File.WriteAllText(Application.dataPath+"/"+id+".txt", json);
    }

    public static void DeletePlayer(string id)
    {
        File.Delete(Application.dataPath+"/"+id+".txt");
    }

    public static void ShowPlayerStat(string id)
    {
        PlayerStruct playerStruct =
            JsonUtility.FromJson<PlayerStruct>(File.ReadAllText(Application.dataPath + "/" + id +
                                                          ".txt"));
        Debug.Log(playerStruct.id + " - " + playerStruct.lvl + " - "+ playerStruct.meshRendererColor);
    }
}
