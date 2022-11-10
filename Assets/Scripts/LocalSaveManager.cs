
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
    public static LocalSaveManager instance;
    private string path;
    private string test;
    private User _user;


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

    public void WriteNewUser()
    {
        _user = new User(MenuLoginManager.instance.GetNickname(), true, true);
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
            WriteNewUser();
        }
        
    }
    
    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(_user);
        Debug.Log(json);
        File.WriteAllText(path+"/savePlayer.json", json);
    }
}
