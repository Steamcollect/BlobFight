using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Window : MonoBehaviour
{
    public enum WindowType
    {
        GameObject,
        Animator,
    }

    [Header("Settings")]
    [SerializeField] private string windowName;
    [SerializeField] private WindowType windowType;

    [Header("References")]
    [SerializeField] private GameObject content;
    [SerializeField] private Animator animator;

    private bool isOpen;
    private Coroutine animationDelay;

    public void EnableWindow()
    {
        content.SetActive(true);

        if (windowType == WindowType.Animator)
        {
            animator.SetBool("IsOpen", true);
        }

        isOpen = true;
    }

    public void DisableWindow()
    {
        if (windowType == WindowType.GameObject)
        {
            content.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
        else if (windowType == WindowType.Animator)
        {
            animator.SetBool("IsOpen", false);

            if (animationDelay != null)
            {
                StopCoroutine(animationDelay);
            }

            animationDelay = StartCoroutine(DisablePanelOnAnimationEnd());
        }

        isOpen = false;
    }

    public string GetName()
    {
        return windowName;
    }

    public bool IsOpen() 
    { 
        return isOpen; 
    }

    private IEnumerator DisablePanelOnAnimationEnd()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        EventSystem.current.SetSelectedGameObject(null);
        content.SetActive(false);
    }
}