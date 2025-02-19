using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MovingProps : GameProps
{
    [Header("Settings")]
    [SerializeField] float moveSpeed;
    int currentPosIndex;

    [SerializeField] float delayBeforeStart;
    [SerializeField] float delayAtPoint;

    [Header("References")]
    [SerializeField] Transform movable;
    [SerializeField] Transform[] positions;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public override void Launch()
    {
        if(positions.Length > 0)
        {
            movable.position = positions[0].position;
            StartCoroutine(DelayBeforeStart());
        }
    }

    IEnumerator DelayBeforeStart()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        SetNextPos();
    }
    IEnumerator DelayAtPoint()
    {
        yield return new WaitForSeconds(delayAtPoint);
        SetNextPos();
    }

    void SetNextPos()
    {
        currentPosIndex = (currentPosIndex + 1) % positions.Length;
        float moveTime = Vector2.Distance(movable.position, positions[currentPosIndex].position) / moveSpeed;
        movable.DOMove(positions[currentPosIndex].position, moveTime).OnComplete(() =>
        {
            StartCoroutine(DelayAtPoint());
        });
    }

    private void OnDestroy()
    {
        movable.DOKill();
    }
}