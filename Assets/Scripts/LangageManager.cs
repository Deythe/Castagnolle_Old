using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangageManager : MonoBehaviour
{
    private SentencesScriptable tradActuel;
    private List<SentencesScriptable> listTrad;

    public void SetTraduction(int i)
    {
        tradActuel = listTrad[i];
    }
}
