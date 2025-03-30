using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlobReadyValidationPanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 posOffset;
    [SerializeField] private float timeToValid;

    [Header("References")]
    [SerializeField] private EntityInput input;
    [SerializeField] private BlobPhysics blobPhysics;
    [SerializeField] private Image wheelImage;
    [SerializeField] private TMP_Text pressInputTxt;
    [SerializeField] private TMP_Text readyTxt;

    [Header("Input")]
    [SerializeField] private RSE_OnGameStart rseOnGameStart;

    [Header("Output")]
    [SerializeField] private RSE_OnBlobReady rseOnBlobReady;

    private Camera cam = null;
    private float currentTime = 0;
    private bool isInputClick = false;
    private bool isValid = false;

    private void OnEnable()
    {
        input.returnInput += ReturnButton;
        input.validateInput += SetValidInput;

        rseOnGameStart.action += DisableGO;
    }

    private void OnDisable()
    {
        input.returnInput -= ReturnButton;
        input.validateInput -= SetValidInput;

        rseOnGameStart.action -= DisableGO;
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!isValid && blobPhysics.GetRigidbody().bodyType != RigidbodyType2D.Static)
        {
            ValidateInputProgress();
        }
        else
        {
            isInputClick = false;
        }

        UpdateWheelPosition();
    }

    private void ValidateInputProgress()
    {
        if (isInputClick)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= timeToValid)
            {
                isValid = true;
                pressInputTxt.gameObject.SetActive(false);
                wheelImage.gameObject.SetActive(false);
                readyTxt.gameObject.SetActive(true);
                transform.BumpVisual();

                rseOnBlobReady.Call();
            }
        }
        else
        {
            currentTime -= Time.deltaTime * 5;
            if (currentTime < 0) currentTime = 0;
        }

        UpdateWheelAmount();
    }

    private void SetValidInput(bool isClick)
    {
        if (blobPhysics.GetRigidbody().bodyType != RigidbodyType2D.Static)
        {
            isInputClick = isClick;
        }
    }

    private void ReturnButton()
    {
        isValid = false;
        readyTxt.gameObject.SetActive(false);
        wheelImage.gameObject.SetActive(true);
        pressInputTxt.gameObject.SetActive(true);
        
        transform.BumpVisual();
    }

    private void UpdateWheelAmount()
    {
        wheelImage.fillAmount = currentTime / timeToValid;
    }

    private void UpdateWheelPosition()
    {
        transform.position = cam.WorldToScreenPoint(blobPhysics.GetCenter() + posOffset);
    }

    private void DisableGO()
    {
        gameObject.SetActive(false);
    }

    public bool IsReady() { return isValid; }
}