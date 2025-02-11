using UnityEngine;
using DG.Tweening;

public class MovingProps : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float moveSpeed;
    int currentPosIndex;

    [Header("References")]
    [SerializeField] Transform movable;
    [SerializeField] Transform[] positions;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        if(positions.Length > 0)
        {
            movable.position = positions[0].position;
            SetNextPos();
        }
    }

    void SetNextPos()
    {
        currentPosIndex = (currentPosIndex + 1) % positions.Length;
        float moveTime = Vector2.Distance(movable.position, positions[currentPosIndex].position) / moveSpeed;
        movable.DOMove(positions[currentPosIndex].position, moveTime).OnComplete(() =>
        {
            SetNextPos();
        });
    }
}