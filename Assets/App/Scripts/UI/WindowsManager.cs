using UnityEngine;
using System.Collections;
using System.Linq;

public class WindowsManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Window[] windows;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_EnableWindow rseEnableWindow;
    [SerializeField] RSE_DisableWindow rseDisableWindow;
    [SerializeField] RSE_CloseAllWindow rseCloseAllWindow;
    //[Header("Output")]

    private void OnEnable()
    {
        rseEnableWindow.action += EnableWindow;
        rseDisableWindow.action += DisableWindow;
        rseCloseAllWindow.action += CloseAllWindow;
    }
    private void OnDisable()
    {
        rseEnableWindow.action -= EnableWindow;
        rseDisableWindow.action -= DisableWindow;
        rseCloseAllWindow.action -= CloseAllWindow;
    }

    void EnableWindow(string windowName)
    {
        Window window = windows.FirstOrDefault(x => x.windowName == windowName);
        if (window != null)
        {
            window.EnableWindow();
        }
    }
    void DisableWindow(string windowName)
    {
        Window window = windows.FirstOrDefault(x => x.windowName == windowName);
        if (window != null)
        {
            window.DisableWindow();
        }
    }
    void CloseAllWindow()
    {
        foreach (var window in windows)
        {
            if (window.IsOpen())
            {
                window.DisableWindow();
            }
        }
    }
}