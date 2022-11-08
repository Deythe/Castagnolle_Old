using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase != TouchPhase.Ended )
            {
                Debug.Log("Test");
                Page3.instance.p_inDropZone = true;
            }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase != TouchPhase.Ended)
            {
                Debug.Log("Papa");
                Page3.instance.p_inDropZone = false;
            }
        }
    }
}
