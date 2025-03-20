using System.Collections;
using UnityEngine;
public class BlobStamina : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxStamina;
    float currentStamina;

    [SerializeField] float staminaGivenPerSec;
    [SerializeField] float waitingTimeWhenNoStamina;

    bool isWaitingForStamina = false;
    bool canGetStamina = true;

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
        if (!canGetStamina || isWaitingForStamina) return;

        if (currentStamina > maxStamina)
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
        if (currentStamina <= 1)
        {
            currentStamina = 0;
            StartCoroutine(OnNoStaminaDelay());
        }
    }

    IEnumerator OnNoStaminaDelay()
    {
        isWaitingForStamina = true;
        yield return new WaitForSeconds(waitingTimeWhenNoStamina);
        isWaitingForStamina = false;
    }

    public bool HaveEnoughStamina(float staminaRequire) { return currentStamina >= staminaRequire; }
    public float GetStamina() {  return currentStamina; }
    public void EnableStaminaRecuperation() { canGetStamina = true;}
    public void DisableStaminaRecuperation() { canGetStamina = false;}
}