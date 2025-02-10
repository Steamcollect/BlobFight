using UnityEngine;
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
        else
        {
            Debug.LogError($"There is no window of name \"{windowName}\"");
        }
    }
    void DisableWindow(string windowName)
    {
        Window window = windows.FirstOrDefault(x => x.windowName == windowName);
        if (window != null)
        {
            window.DisableWindow();
        }
        else
        {
            Debug.LogError($"There is no window of name \"{windowName}\"");
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

[System.Serializable]
public class Window
{
    public string windowName;

    public GameObject content;
    bool isOpen;

    public void EnableWindow()
    {
        content.SetActive(true);
        isOpen = true;
    }
    public void DisableWindow()
    {
        content.SetActive(false);
        isOpen = false;
    }
    public bool IsOpen() { return isOpen; }
}