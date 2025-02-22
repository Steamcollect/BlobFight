using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonNavigationGamepad : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] GameObject defaultButton;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Update()
    {
        if (Input.GetButtonDown("Submit") || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(defaultButton);
            }
        }
    }
}