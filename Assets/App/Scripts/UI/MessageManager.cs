using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panelMessage;
    [SerializeField] private TextMeshProUGUI textMessage;

    [Header("Input")]
    [SerializeField] private RSE_Message rseMessage;
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private bool isPaused = false;

    private void OnEnable()
    {
        rseMessage.action += ShowMessage;
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }

    private void OnDisable()
    {
        rseMessage.action -= ShowMessage;
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    private void ShowMessage(string text, float duration, Color textColor)
    {
        textMessage.text = text;
        textMessage.color = textColor;
        panelMessage.SetActive(true);

        StartCoroutine(DelayHideMessage(duration));
    }

    private IEnumerator DelayHideMessage(float duration)
    {
        float cooldown = duration;
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
            }
        }

        panelMessage.SetActive(false);
    }
}
