using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuDeckManager : MonoBehaviour
{
    [SerializeField] private GameObject canvasDice;
    [SerializeField] private GameObject canvasCard;

    public void ChangeCanvasUi()
    {
        canvasCard.SetActive(!canvasCard.activeSelf); 
        canvasDice.SetActive(!canvasDice.activeSelf);
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
