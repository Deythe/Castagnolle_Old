using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardData : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private Sprite card;
    [SerializeField] private Sprite bigCard;
    [SerializeField] private GameObject prefabs;
    [SerializeField] private int atk;
    [SerializeField] private List<int> resources;
    [SerializeField] private bool isChampion;
    
    private RectTransform rec;
    private float initalYPosition;
    private bool waiting;
    
    public int Atk
    {
        get => atk;
        set
        {
            atk = value;
        }
    }
    
    public GameObject Prefabs
    {
        get => prefabs;
        set
        {
            prefabs = value;
        }
    }
    
    public bool IsChampion
    {
        get => isChampion;
        set
        {
            isChampion = value;
        }
    }
    
    public List<int> Ressources
    {
        get => resources;
        set
        {
            resources = value;
        }
    }
    
    private void Start()
    {
        rec = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                ReInit();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (waiting)
            {
                ReInit();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                initalYPosition = transform.localPosition.y;
                UiManager.instance.AbleUpdateCard(bigCard);
                waiting = true;
                StopAllCoroutines();
                StartCoroutine(CoroutineShow());
            }
        }
    }

    private void ReInit()
    {
        waiting = false;
        UiManager.instance.ShowingOffBigCard();
        rec.localPosition = new Vector3(rec.localPosition.x, 0, rec.localPosition.z);
        GetComponentInParent<ScrollRect>().horizontal = true;
        UiManager.instance.Card = null;
    }

    IEnumerator CoroutineShow()
    {
        yield return new WaitForSeconds(0.5f);
        if (waiting)
        {
            waiting = false;
            GetComponentInParent<ScrollRect>().horizontal = false;
            UiManager.instance.Card = gameObject;
        }
    }
}
