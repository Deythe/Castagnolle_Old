
using System;
using System.IO;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LocalSaveManager : MonoBehaviour
{
    public static LocalSaveManager instance;
    
    public UnitListScriptable _unitList;
    public DiceListScriptable _dicesList;
    
    private string path;
    private string test;
    private User _user;
    
    public UnitListScriptable unitList
    {
        get => _unitList;
    }

    public DiceListScriptable dicesList
    {
        get => _dicesList;
    }
    
    public User user
    {
        get => _user;
        set
        {
            _user = value;
        }
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
        path = Application.dataPath;
    }

    public void WriteNewUser(string pseudo)
    {
        _user = new User(pseudo, true, true);
        SaveUserData();
    }

    public void ReadLocalUser()
    {
        try
        {
            test = File.ReadAllText(path + "/savePlayer.json");
            Debug.Log(test);
            _user = JsonUtility.FromJson<User>(test);
        }
        catch (Exception e)
        {
            WriteNewUser("Visiteur");
        }
        
    }
    
    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(_user);
        Debug.Log(json);
        File.WriteAllText(path+"/savePlayer.json", json);
    }
}
