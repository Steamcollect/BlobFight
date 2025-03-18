using System;
using UnityEngine;
public class WarningMovingProps : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool haveWarning;
    [SerializeField] bool isOnAxeX;
    [SerializeField] float marginPos;
    bool isVisible = false;
    [Header("References")]
    private Camera cam;
    [SerializeField] GameObject warning;
    [SerializeField] Transform movable;
    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        cam = Camera.main;
    }
    private void Start()
    {
        if(haveWarning)
        {
            UpdateWarning(!Checkisibility());
        }

    }
    private void FixedUpdate()
    {
        bool visible = Checkisibility();
        if (haveWarning && visible != isVisible)
        {
            isVisible = visible;
           UpdateWarning(!visible);
        }
    }
    bool Checkisibility()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(planes, new Bounds(movable.position, Vector3.one));
    }
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
        Vector3 sceneDimension = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Debug.DrawLine(sceneDimension, Vector3.zero,Color.black,1);

        float posX = Mathf.Clamp(movable.position.x, cam.transform.position.x - sceneDimension.x, cam.transform.position.x + sceneDimension.x);
        float posY = Mathf.Clamp(movable.position.y, cam.transform.position.y - sceneDimension.y, cam.transform.position.y + sceneDimension.y);

        if (isOnAxeX)
        {
            if(movable.transform.position.x < 0)
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
            if (movable.transform.position.y < 0)
            {
                warning.transform.position = new Vector3(posX, posY + marginPos, movable.position.z);
            }
            else
            {
                warning.transform.position = new Vector3(posX, posY - marginPos, movable.position.z);
            }
        }
    }
}