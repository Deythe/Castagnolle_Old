using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoginManager : MonoBehaviour
{
    public static MenuLoginManager instance;
    [SerializeField] private string bugExplain;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject LoginMenu;
    [SerializeField] private GameObject CreateMenu;
    [SerializeField] private string nickname;
    [SerializeField] private bool bug;
    
    private bool menuTime;
    private TouchScreenKeyboard keyboard;

    private void Awake()
    {
        instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Update()
    {
        if (bug)
        {
            bug = false;
            text.text = bugExplain;
        }

        if (FireBaseManager.instance.Create && !menuTime)
        {
            menuTime = true;
            LoginMenu.SetActive(false);
            CreateMenu.SetActive(true);
        }

        if (!FireBaseManager.instance.Create && FireBaseManager.instance.UserFireBase != null)
        {
            if (FireBaseManager.instance.User != null)
            {
                PhotonNetwork.ConnectUsingSettings();
                FireBaseManager.instance.User.isConnected = true;
                SceneManager.LoadScene(1);
            }
            else
            {
                FireBaseManager.instance.User = JsonUtility.FromJson<User>(FireBaseManager.instance.GetStringRequest());
                FireBaseManager.instance.OnConnected();
            }
        }
    }
    
    public void SetNickname(string value)
    {
        nickname = value;
    }

    public void OpenKeyBoard(bool b)
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, b);
    }

    
    public void SetBugExplain(string p)
    {
        bugExplain = p;
    }

    public string GetNickname()
    {
        return nickname;
    }

    public void SetBug(bool b)
    {
        bug = b;
    }





}
