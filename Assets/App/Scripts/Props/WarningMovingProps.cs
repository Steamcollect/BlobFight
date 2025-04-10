using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class WarningMovingProps : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool haveWarning;
    [SerializeField] private bool isOnAxeX;
    [SerializeField] private float marginPos;
    [SerializeField] private Vector3 offsetPos;
    [SerializeField] private bool doFlashWarning = true;
    [SerializeField] private float delay = 1.0f;
    [SerializeField] private float minDelay = 0.1f;
    [SerializeField] private float decreaseRate = 0.05f;

    [Header("References")]
    [SerializeField] private GameObject warning;
    [SerializeField] private Transform movable;

    public Action<bool> onWarning;
    private Camera cam;
    private bool warningState;
    
    private Coroutine warningCoroutine = null;
    private float initDelay;

    private void OnEnable()
    {
        onWarning += ActiveWarning;
    }

    private void OnDisable()
    {
        onWarning -= ActiveWarning;
    }

    private void Awake()
    {
        cam = Camera.main;
        initDelay = delay;
    }

    private void Update()
    {
        if (haveWarning)
        {
            if (warningState)
            {
                UpdateWarning(true);
                warningState = false;
            }
            else
            {
                UpdateWarning(false);
            }
        }
    }

    private void UpdateWarning(bool seeWarning)
    {
        if(seeWarning)
        {
            SetPositionWarning();
            if (doFlashWarning)
            {
                warningCoroutine = StartCoroutine(FlashWarning());
                doFlashWarning = false;
            }
        }
        else
        {
            if(warningCoroutine != null)
            {
                if (!doFlashWarning)
                {
                    StopCoroutine(warningCoroutine);
                    warning.SetActive(false);
                    delay = initDelay;
                    doFlashWarning = true;
                }
                warningCoroutine = null;
            }
        }
    }

    private void SetPositionWarning()
    {
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;
        Vector3 camPos = cam.transform.position;
        Vector3 pos = movable.position + offsetPos;

        float halfWidth = camWidth / 2f;
        float halfHeight = camHeight / 2f;

        float posX = Mathf.Clamp(pos.x, camPos.x - halfWidth, camPos.x + halfWidth);
        float posY = Mathf.Clamp(pos.y, camPos.y - halfHeight, camPos.y + halfHeight);

        Vector3 offset = Vector3.zero;

        if (isOnAxeX)
        {
            if (movable.position.x < cam.transform.position.x)
            {
                offset = new Vector3(posX + marginPos, posY, movable.position.z);
            }    
            else
            {
                offset = new Vector3(posX - marginPos, posY, movable.position.z);
            }     
        }
        else
        {
            if (movable.position.y < cam.transform.position.y)
            {
                offset = new Vector3(posX, posY + marginPos, movable.position.z);
            }
            else
            {
                offset = new Vector3(posX, posY - marginPos, movable.position.z);
            }
        }

        warning.transform.position = offset;
    }

    private void ActiveWarning(bool warning)
    {
        warningState = warning;
    } 
    IEnumerator FlashWarning()
    {
        warning.SetActive(true);
        yield return new WaitForSeconds(delay);
        warning.SetActive(false);
        yield return new WaitForSeconds(delay);

        delay = Mathf.Max(minDelay, delay - decreaseRate);
        warningCoroutine = StartCoroutine(FlashWarning());
    }
}