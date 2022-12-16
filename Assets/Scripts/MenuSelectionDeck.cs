using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelectionDeck : MonoBehaviour
{
    [SerializeField] private List<GameObject> cardDeckList, diceDeckList;
    [SerializeField] private Transform middleMenu;
    [SerializeField] private CanvasScaler canvasScaler;
    
    public void GoToDiceDeck()
    {
        middleMenu.DOMoveX(-canvasScaler.referenceResolution.x, 0.2f);
    }
}
