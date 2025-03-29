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

    private void OnEnable()
    {
        rseMessage.action += ShowMessage;
    }

    private void OnDisable()
    {
        rseMessage.action -= ShowMessage;
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
        yield return new WaitForSeconds(duration);

        panelMessage.SetActive(false);
    }
}
