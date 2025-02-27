using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ButtonNavigationGamepad : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] Button defaultButton;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            defaultButton.Select();
        }
    }
}