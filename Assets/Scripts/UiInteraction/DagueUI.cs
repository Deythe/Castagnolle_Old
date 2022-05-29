using DG.Tweening;
using UnityEngine;

public class DagueUI : MonoBehaviour
{
    [SerializeField] private float time;
    private float originalYposition;

    private void OnEnable()
    {
        originalYposition = transform.position.y;
        Move();
    }

    private void Move()
    {
        Debug.Log("CacaKipU");
        transform.DOLocalMoveY(transform.position.y + 0.5f, time)
            .OnComplete(() => transform.DOLocalMoveY(originalYposition, time).OnComplete(Move));
    }

    private void OnDisable()
    {
        transform.DOKill(transform);
        transform.position = Vector3.zero;
    }
}
