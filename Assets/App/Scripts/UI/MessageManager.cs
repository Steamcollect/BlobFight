using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panelMessage;
    [SerializeField] private TextMeshProUGUI textMessage;
    [SerializeField] private Animator animator;
    [SerializeField] private TextAudio textAudio;

    [Space(5)]
    [SerializeField] float messageMaxAngle;

    [Header("Input")]
    [SerializeField] private RSE_Message rseMessage;
    [SerializeField] private RSE_AudioMessage rseAudioMessage;
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private bool isPaused = false;
    private Coroutine animationDelay;
    private bool isVictoryText = false;

    private void OnEnable()
    {
        rseMessage.action += ShowMessage;
        rseAudioMessage.action += CanPlayVictorySound;
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }

    private void OnDisable()
    {
        rseMessage.action -= ShowMessage;
        rseAudioMessage.action += CanPlayVictorySound;
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
    private void CanPlayVictorySound(bool isValid)
    {
        isVictoryText = isValid;
    }
    private void ShowMessage(string text, float duration, Color textColor)
    {
        print(isVictoryText);
        if (isVictoryText)
        {
            print("Win");
            textAudio.PlayVictorySound();
        }
        else
        {
            print("Start");
            textAudio.PlayStartSound();
        }
        textMessage.text = text;
        textMessage.color = textColor;

        panelMessage.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-messageMaxAngle, messageMaxAngle));
        
        panelMessage.SetActive(true);
        animator.SetBool("IsFade", true);


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

        animator.SetBool("IsFade", false);

        if (animationDelay != null)
        {
            StopCoroutine(animationDelay);
        }

        animationDelay = StartCoroutine(DelayClose());
    }

    private IEnumerator DelayClose()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        panelMessage.SetActive(false);
    }
}
