using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GhostNumber : MonoBehaviour
{
    [SerializeField] private bool isPlaying = false;
    private WaitForSeconds time = new WaitForSeconds(3);

    public bool p_isPlaying
    {
        get => isPlaying;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        isPlaying = true;
        transform.DOMoveY(transform.position.y + 3, 3f);
        transform.GetComponent<TMP_Text>().DOFade(0, 2f);
        yield return time;
        isPlaying = false;
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        transform.GetComponent<TMP_Text>().DOFade(1,0f);
    }
}
