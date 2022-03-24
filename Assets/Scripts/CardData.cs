using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardData : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private MonsterCardScriptable stats;
    [SerializeField] private Image card;
    
    private float initalYPosition;
    private bool waiting;
    private void Start()
    {
        initalYPosition = GetComponent<RectTransform>().position.y;
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

    public MonsterCardScriptable GetStat()
    {
        return stats;
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
                UiManager.instance.AbleUpdateCard(card);
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
        transform.position = new Vector3(transform.position.x, initalYPosition, transform.position.z);
        GetComponentInParent<ScrollRect>().horizontal = true;
        UiManager.instance.SetCard(null);
    }

    IEnumerator CoroutineShow()
    {
        yield return new WaitForSeconds(0.5f);
        if (waiting)
        {
            waiting = false;
            GetComponentInParent<ScrollRect>().horizontal = false;
            UiManager.instance.SetCard(gameObject);
        }
    }

    public float GetInitialPositionY()
    {
        return initalYPosition;
    }
}
