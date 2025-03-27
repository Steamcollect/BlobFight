using System;
using UnityEngine;
public class WarningMovingProps : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool haveWarning;
    [SerializeField] bool isOnAxeX;
    [SerializeField] float marginPos;
    //bool isVisible = false;
    [Header("References")]
    private Camera cam;
    [SerializeField] GameObject warning;
    [SerializeField] Transform movable;
    public Action<bool> onWarning;
    public bool warningState;
    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
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
    }
    //private void Start()
    //{
    //    if(haveWarning)
    //    {
    //        UpdateWarning(!Checkisibility());
    //    }

    //}
    private void Update()
    {
        //bool visible = Checkisibility();
        
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
    //bool Checkisibility()
    //{
    //    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
    //    return GeometryUtility.TestPlanesAABB(planes, new Bounds(movable.position, Vector3.one));
    //}
    private void UpdateWarning(bool seeWarning)
    {
        if(seeWarning)
        {            
            SetPositionWarning();
            warning.SetActive(true);
        }
        else
        {
            warning.SetActive(false);
        }
    }
    private void SetPositionWarning()
    {
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        float halfWidth = camWidth / 2f;
        float halfHeight = camHeight / 2f;

        float posX = Mathf.Clamp(movable.position.x, cam.transform.position.x - halfWidth, cam.transform.position.x + halfWidth);
        float posY = Mathf.Clamp(movable.position.y, cam.transform.position.y - halfHeight, cam.transform.position.y + halfHeight);

        if (isOnAxeX)
        {
            if (movable.position.x < cam.transform.position.x)
            {
                warning.transform.position = new Vector3(posX + marginPos, posY, movable.position.z);
            }    
            else
            {
                warning.transform.position = new Vector3(posX - marginPos, posY, movable.position.z);
            }     
        }
        else
        {
            if (movable.position.y < cam.transform.position.y)
            {
                warning.transform.position = new Vector3(posX, posY + marginPos, movable.position.z);
            }
            else
            {
                warning.transform.position = new Vector3(posX, posY - marginPos, movable.position.z);
            }
        }
    }
    private void ActiveWarning(bool warning)
    {
        warningState = warning;
    }
}