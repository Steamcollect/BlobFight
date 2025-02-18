using UnityEngine;
public class BlobStamina : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxStamina;
    float currentStamina;

    [SerializeField] float staminaGivenPerSec;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public void Setup()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if(currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        else
        {
            currentStamina += staminaGivenPerSec * Time.deltaTime;
        }
    }

    public void RemoveStamina(float stamina)
    {
        currentStamina -= stamina;
        if (currentStamina < 0) currentStamina = 0;
    }

    public bool HaveEnoughStamina(float staminaRequire) { return currentStamina >= staminaRequire; }
    public float GetStamina() {  return currentStamina; }
}