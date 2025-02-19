using UnityEngine;
public abstract class GameProps : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GamePropsLaunchType launchType;

    enum GamePropsLaunchType
    {
        OnStart,
        OnFightStart,
        OnEvent
    }

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnFightStart rseOnFightStart;

    //[Header("Output")]

    private void OnEnable()
    {
        if (launchType == GamePropsLaunchType.OnFightStart) rseOnFightStart.action += Launch;
    }
    private void OnDisable()
    {
        if (launchType == GamePropsLaunchType.OnFightStart) rseOnFightStart.action -= Launch;
    }

    private void Start()
    {
        if (launchType == GamePropsLaunchType.OnStart) Launch();
    }

    public abstract void Launch();
}