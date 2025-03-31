using System.Collections;
using UnityEngine;
public class BlobStamina : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaGivenPerSec;
    [SerializeField] private float waitingTimeWhenNoStamina;

    private float currentStamina = 0;
    private bool isWaitingForStamina = false;
    private bool canGetStamina = true;

    public void Setup()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (!canGetStamina || isWaitingForStamina) return;

        currentStamina = Mathf.Min(currentStamina + staminaGivenPerSec * Time.deltaTime, maxStamina);
    }

    public void RemoveStamina(float amount)
    {
        currentStamina = Mathf.Max(currentStamina - amount, 0);

        if (currentStamina == 0)
        {
            StartCoroutine(OnNoStaminaDelay());
        }
    }

    private IEnumerator OnNoStaminaDelay()
    {
        isWaitingForStamina = true;
        yield return new WaitForSeconds(waitingTimeWhenNoStamina);
        isWaitingForStamina = false;
    }

    public bool HaveEnoughStamina(float staminaRequire) { return currentStamina >= staminaRequire; }
    public bool HaveEnoughStaminaPercentage(float percentage) { return currentStamina / maxStamina > percentage; }

    public float GetStamina() {  return currentStamina; }
    public float GetStaminaPercentage() { return currentStamina / maxStamina; }

    public void EnableStaminaRecuperation() { canGetStamina = true;}

    public void DisableStaminaRecuperation() { canGetStamina = false;}
}