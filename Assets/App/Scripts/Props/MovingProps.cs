using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class MovingProps : GameProps
{
    [Space(10)]
    [Header("Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool notHide;
    [SerializeField] private float delayBeforeStart;
    [SerializeField] private float delayAtPoint;
    [SerializeField] private float delayWarningBeforeMove;
    [SerializeField] private List<int> timeSpeed;
    [SerializeField] private List<float> newDelayAtPoint;
    [SerializeField] private List<float> newMoveSpeed;

    [Space(10)]
    [SerializeField] bool shakeBeforeMoving;
    [SerializeField] float shakeForce;
    [SerializeField] float shakeDuration;

    [Space(5)]
    [SerializeField] bool shakeDuringMovement;
    [SerializeField] float shakeDuringMovementForce;

    [Header("References")]
    [SerializeField] private Transform movable;
    [SerializeField] private WarningMovingProps warningMovingProps;
    [SerializeField] private Transform[] positions;
    [SerializeField] ParticleSystem[] particleDuringMovement;

    [Header("Input")]
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    [Header("Output")]
    [SerializeField] RSO_TimerParty rsoTimerParty;

    private bool isPaused = false;
    private int currentPosIndex = 0;
    private int mode = 0;
    private Coroutine coroutine;

    private new void OnEnable()
    {
        base.OnEnable();
        rseOnFightEnd.action += StopDelay;
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }

    private new void OnDisable()
    {
        base.OnDisable();
        rseOnFightEnd.action -= StopDelay;
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    private void OnDestroy()
    {
        movable.DOKill();
    }

    private void Pause()
    {
        isPaused = true;
        movable.DOPause();
    }

    private void Resume()
    {
        isPaused = false;
        movable.DOPlay();
    }

    public override void Launch()
    {
        if (positions.Length > 0)
        {
            movable.position = positions[0].position;
            coroutine = StartCoroutine(DelayLaunch(delayBeforeStart));
        }
    }

    private void StopDelay()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator DelayLaunch(float delay)
    {
        float cooldown = delay;
        float timer = 0f;

        if (shakeBeforeMoving)
        {
            StartCoroutine(Utils.Delay(delay - shakeDuration, () =>
            {
                movable.DOPunchRotation(Vector3.forward * shakeForce, shakeDuration, 20, 1);
            }));
        }

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
                if (timer > Mathf.Max(delay - delayWarningBeforeMove, 0))
                {
                    warningMovingProps?.onWarning.Invoke(true);
                }
            }
        }

        SetNextPos();
    }

    private void SetNextPos()
    {
        if (!notHide)
        {
            movable.gameObject.SetActive(true);
        }
		
		currentPosIndex = (currentPosIndex + 1) % positions.Length;
        float moveTime = Vector2.Distance(movable.position, positions[currentPosIndex].position) / moveSpeed;

        if (timeSpeed.Count > 0)
        {
            if (rsoTimerParty.Value >= timeSpeed[mode] && mode < timeSpeed.Count)
            {
                delayAtPoint = newDelayAtPoint[mode];
                moveSpeed = newMoveSpeed[mode];

                if (mode < timeSpeed.Count - delayWarningBeforeMove)
                {
                    mode++;
                }
            }
        }

        for (int i = 0; i < particleDuringMovement.Length; i++)
        {
            particleDuringMovement[i].Play();
        }

        movable.DOKill();
        if (shakeDuringMovement)
        {
            movable.DOPunchRotation(Vector3.forward * shakeDuringMovementForce, moveTime, 20, 1);
        }
        movable.DOMove(positions[currentPosIndex].position, moveTime).OnComplete(() =>
        {
            if (!notHide)
            {
                movable.gameObject.SetActive(false);
            }

            for (int i = 0; i < particleDuringMovement.Length; i++)
            {
                particleDuringMovement[i].Stop();
            }

            coroutine = StartCoroutine(DelayLaunch(delayAtPoint));
		});
    }
}