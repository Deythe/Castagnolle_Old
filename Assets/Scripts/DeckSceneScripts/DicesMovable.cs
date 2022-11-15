using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DicesMovable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int _index;
    [SerializeField] private Sprite _sprite;
    private Coroutine lastCoroutine;
    public int index
    {
        get => _index;
        set
        {
            _index = value;
        }
    }
    
    public Sprite sprite
    {
        get => _sprite;
        set
        {
            _sprite = value;
        }
    }
    
    private void Start()
    {
        _sprite = GetComponent<Image>().sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                lastCoroutine = StartCoroutine(CoroutineShowCart());
            }
        }
    }
    
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if(lastCoroutine==null || DeckBuildingManager.instance.isMovingACard) return;
            if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                StopCoroutine(lastCoroutine);
                DeckBuildingManager.instance.bigCard.SetActive(false);
                if (Input.GetTouch(0).deltaPosition.x > 4)
                {
                    DeckBuildingManager.instance.currentIndexObjectMovable = index;
                    DeckBuildingManager.instance.isMovingACard = true;
                }
            }
        }
    }

    IEnumerator CoroutineShowCart()
    {
        yield return new WaitForSeconds(0.5f);
        //DeckBuildingManager.instance.bigCard.GetComponent<Image>().sprite = cardFull;
        DeckBuildingManager.instance.bigCard.SetActive(true);
    }
}
