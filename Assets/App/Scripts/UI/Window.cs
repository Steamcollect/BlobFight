using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class Window : MonoBehaviour
{
    public string windowName;
    public WindowType windowType;
    public Button buttonResume;

    [Space(10)]
    public GameObject content;
    public Animator animator;
    bool isOpen;

    Coroutine animationDelay;

    public void EnableWindow()
    {
        switch (windowType)
        {
            case WindowType.GameObject:
                content.SetActive(true);
                break;

            case WindowType.Animator:
                content.SetActive(true);
                animator.SetBool("IsOpen", true);
                break;
        }
        isOpen = true;
    }
    public void DisableWindow()
    {
        switch (windowType)
        {
            case WindowType.GameObject:
                content.SetActive(false);
                break;

            case WindowType.Animator:
                animator.SetBool("IsOpen", false);

                if(animationDelay != null) StopCoroutine(animationDelay);
                animationDelay = StartCoroutine(DisablePanelOnAnimationEnd());
                break;
        }
        isOpen = false;
    }
    public bool IsOpen() { return isOpen; }

    IEnumerator DisablePanelOnAnimationEnd()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        EventSystem.current.SetSelectedGameObject(null);
        content.SetActive(false);
    }

    public enum WindowType
    {
        GameObject,
        Animator,
    }
}