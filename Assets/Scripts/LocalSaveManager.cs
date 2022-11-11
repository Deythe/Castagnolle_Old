
using System;
using System.IO;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LocalSaveManager : MonoBehaviour
{
    [SerializeField] private UnitListScriptable _unitListDeck;
    public static LocalSaveManager instance;
    private string path;
    private string test;
    private User _user;
    
    public UnitListScriptable unitListScriptable
    {
        get => _unitListDeck;
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
