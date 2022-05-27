using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FireBaseManager.instance.User.firstTime = false;
        if (FireBaseManager.instance.User.isConnected)
        {
            FireBaseManager.instance.FirstTimeChecked();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
