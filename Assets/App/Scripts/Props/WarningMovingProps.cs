using UnityEngine;
public class WarningMovingProps : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] GameObject warning;
    [SerializeField] RSE_UpdateWarning RSE_UpdateWarning;
    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private void OnEnable()
    {
        RSE_UpdateWarning.action += UpdateWarning;
    }
    private void OnDisable()
    {
        RSE_UpdateWarning.action -= UpdateWarning;
    }
    private void UpdateWarning(bool seeWarning)
    {
        if(seeWarning)
        {
            warning.SetActive(true);
        }
        else
        {
            warning.SetActive(false);
        }
    }
}