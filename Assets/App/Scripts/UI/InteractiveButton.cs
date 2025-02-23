using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class InteractiveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Settings")]
    [SerializeField] float selectionPunchForce;
    [SerializeField] float selectionPunchDuration;
    [Space(5)]
    [SerializeField] float hoverideScale;
    [SerializeField] float hoverideScaleTime;

    [Space(20)]
    [SerializeField] float deselectScaleTime;

    [Space(20)]
    [SerializeField] Color hoverColor;
    [SerializeField] Color initColor;

    [Header("References")]
    [SerializeField] Transform interactiveContent;
    [SerializeField] TMP_Text text;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelected();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselected();
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelected();
    }
    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselected();
    }

    void Start()
    {
        text.color = initColor;
    }

    void OnSelected()
    {
        interactiveContent.DOKill();
        interactiveContent.rotation = Quaternion.identity;

        interactiveContent.DOPunchRotation(Vector3.forward * selectionPunchForce, selectionPunchDuration, 20, 1);
        interactiveContent.DOScale(hoverideScale, hoverideScaleTime);

        text.DOKill();
        text.DOColor(hoverColor, hoverideScaleTime);
    }
    void OnDeselected()
    {
        interactiveContent.DOKill();
        interactiveContent.rotation = Quaternion.identity;
        interactiveContent.DOScale(1, deselectScaleTime);

        text.DOKill();
        text.DOColor(initColor, deselectScaleTime);
    }
}