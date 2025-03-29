using UnityEngine;
using System.Linq;

public class WindowsManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Window[] windows;

    [Header("Input")]
    [SerializeField] private RSE_EnableWindow rseEnableWindow;
    [SerializeField] private RSE_DisableWindow rseDisableWindow;
    [SerializeField] private RSE_CloseAllWindow rseCloseAllWindow;

    [Header("Output")]
    [SerializeField] private RSE_EnableJoining rseEnableJoining;

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

    private void Start()
    {
        rseEnableJoining.Call();
    }

    private void EnableWindow(string windowName)
    {
        Window window = windows.FirstOrDefault(x => x.GetName() == windowName);

        if (window != null)
        {
            window.EnableWindow();
        }
    }

    private void DisableWindow(string windowName)
    {
        Window window = windows.FirstOrDefault(x => x.GetName() == windowName);

        if (window != null)
        {
            window.DisableWindow();
        }
    }

    private void CloseAllWindow()
    {
        foreach (var window in windows.Where(w => w.IsOpen()))
        {
            window.DisableWindow();
        }
    }
}