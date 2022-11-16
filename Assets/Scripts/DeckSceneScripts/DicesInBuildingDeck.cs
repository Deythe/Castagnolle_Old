using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DicesInBuildingDeck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
                DeckBuildingManager.instance.diceDetails.gameObject.SetActive(false);
                DeckBuildingManager.instance.diceDetails.DOScaleX(0, 0);
                if ((_index % 2).Equals(0))
                {
                    DeckBuildingManager.instance.diceListDisplayContent.GetChild(_index + 1).gameObject.SetActive(true);
                }
                
                if (Input.GetTouch(0).deltaPosition.x > 4)
                {
                    DeckBuildingManager.instance.cardMovableSprite = _sprite;
                    DeckBuildingManager.instance.currentIndexObjectMovable = index;
                    DeckBuildingManager.instance.isMovingACard = true;
                }
            }
        }
    }

    IEnumerator CoroutineShowCart()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < LocalSaveManager.instance.dicesList.dicesList[_index].faces.Count; i++)
        {
            DeckBuildingManager.instance.diceDetails.GetChild(0).GetChild(i).GetComponent<Image>().sprite =
                LocalSaveManager.instance.dicesList.ChooseGoodSprite(LocalSaveManager.instance.dicesList.dicesList[_index].faces, i);
        }
        DeckBuildingManager.instance.diceDetails.position = transform.position + new Vector3(80,0,0);
        DeckBuildingManager.instance.diceDetails.DOScaleX(1, 0.25f);
        if ((_index % 2).Equals(0))
        {
            DeckBuildingManager.instance.diceListDisplayContent.GetChild(_index + 1).gameObject.SetActive(false);
        }
        DeckBuildingManager.instance.diceDetails.gameObject.SetActive(true);
    }
}
