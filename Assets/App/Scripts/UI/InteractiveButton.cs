using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

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
    //[Header("References")]

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

    void OnSelected()
    {
        transform.DOKill();
        transform.rotation = Quaternion.identity;

        transform.DOPunchRotation(Vector3.forward * selectionPunchForce, selectionPunchDuration, 20, 1);
        transform.DOScale(hoverideScale, hoverideScaleTime);
    }
    void OnDeselected()
    {
        transform.DOKill();
        transform.rotation = Quaternion.identity;
        transform.DOScale(1, deselectScaleTime);
    }
}