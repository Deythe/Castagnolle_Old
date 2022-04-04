
using System;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class FireBaseManager : MonoBehaviour
{
    public static FireBaseManager instance;
    
    private User user;
    private string stringRequest;
    
    private FirebaseAuth auth; 
    private FirebaseUser userFirebase;
    private DatabaseReference root ;

    [SerializeField] private bool create;
    [SerializeField] private string email;
    [SerializeField] private string password;

    public void SetEmail(string value)
    {
        email = value;
    }

    public void SetPassword(string value)
    {
        password = value;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
        auth = FirebaseAuth.DefaultInstance;
        root = FirebaseDatabase.GetInstance("https://summoners-dice-default-rtdb.europe-west1.firebasedatabase.app/")
            .RootReference;
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                MenuLoginManager.instance.SetBugExplain(task.Exception.GetBaseException().Message);
                MenuLoginManager.instance.SetBug(true);
                return;
            }
            
            userFirebase = task.Result;
            
            root.Child("users").Child(userFirebase.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted) {
                    Debug.Log("non");
                }
                else if (task.IsCompleted) {
                    DataSnapshot snapshot = task.Result;
                    stringRequest = snapshot.GetRawJsonValue();
                }
            });
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                userFirebase.DisplayName, userFirebase.UserId);
        });
    }

    public void WriteNewUser()
    {
        int[] tab = new int[20];
        for (int i = 0; i < 20; i++)
        {
            tab[i] = 1;
        }
        user = new User(MenuLoginManager.instance.GetNickname(), true, tab);
        string json = JsonUtility.ToJson(user);
        root.Child("users").Child(userFirebase.UserId).SetRawJsonValueAsync(json);
        create = false;
    }
    
    public void OnConnected()
    {
        root.Child("users").Child(userFirebase.UserId).Child("isConnected").SetValueAsync(true);
    }

    private void OnApplicationQuit()
    {
        root.Child("users").Child(userFirebase.UserId).Child("isConnected").SetValueAsync(false);
    }

    public void CreateUser()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                MenuLoginManager.instance.SetBugExplain(task.Exception.GetBaseException().Message);
                MenuLoginManager.instance.SetBug(true);
                return;
            }

            create = true;
            userFirebase = task.Result;
            
            Debug.LogFormat("Firebase userFirebase created successfully: {0} ({1})",
                userFirebase.DisplayName, userFirebase.UserId);

        });
    }

    public bool GetCreate()
    {
        return create;
    }

    public void SetCreate(bool b)
    {
        create = b;
    }

    public FirebaseUser GetUserFireBase()
    {
        return userFirebase;
    }

    public string GetStringRequest()
    {
        return stringRequest;
    }

    public User GetUser()
    {
        return user;
    }

    public void SetUser(User s)
    {
        user = s;
    }
}
