using UnityEngine;

public abstract class GameProps : MonoBehaviour
{
    private enum GamePropsLaunchType
    {
        OnStart,
        OnFightStart,
        OnEvent
    }

    [Header("Settings")]
    [SerializeField] GamePropsLaunchType launchType;

    [Header("Input")]
    [SerializeField] RSE_OnFightStart rseOnFightStart;

    protected void OnEnable()
    {
        if (launchType == GamePropsLaunchType.OnFightStart) rseOnFightStart.action += Launch;
    }

    protected void OnDisable()
    {
        if (launchType == GamePropsLaunchType.OnFightStart) rseOnFightStart.action -= Launch;
    }

    private void Start()
    {
        if (launchType == GamePropsLaunchType.OnStart) Launch();
    }

    public abstract void Launch();
}