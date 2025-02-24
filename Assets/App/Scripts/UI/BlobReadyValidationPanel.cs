using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlobReadyValidationPanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 posOffset;
    [SerializeField] float timeToValid;
    float currentTime;

    bool isInputClick = false;
    bool isValid = false;

    [Header("References")]
    [SerializeField] EntityInput input;
    [SerializeField] BlobJoint joint;

    Camera cam;

    [Space(10)]
    [SerializeField] Image wheelImage;
    [SerializeField] TMP_Text pressInputTxt;
    [SerializeField] TMP_Text readyTxt;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnGameStart rseOnGameStart;

    [Header("Output")]
    [SerializeField] RSE_OnBlobReady rseOnBlobReady;

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
        if (!isValid)
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

        UpdateWheelPosition();
    }

    void SetValidInput(bool isClick) => isInputClick = isClick;

    void ReturnButton()
    {
        isValid = false;
        readyTxt.gameObject.SetActive(false);
        wheelImage.gameObject.SetActive(true);
        pressInputTxt.gameObject.SetActive(true);
        
        transform.BumpVisual();
    }

    void UpdateWheelAmount()
    {
        wheelImage.fillAmount = currentTime / timeToValid;
    }
    void UpdateWheelPosition()
    {
        transform.position = cam.WorldToScreenPoint(joint.GetJointsCenter() + posOffset);
    }

    void DisableGO()
    {
        gameObject.SetActive(false);
    }

    public bool IsReady() { return isValid; }
}