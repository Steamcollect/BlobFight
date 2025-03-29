using UnityEngine;
using DG.Tweening;
using TMPro;

public class InteractiveButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float selectionPunchForce;
    [SerializeField] private float selectionPunchDuration;
    [SerializeField] private float hoverideScale;
    [SerializeField] private float hoverideScaleTime;
    [SerializeField] private float deselectScaleTime;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color initColor;

    [Header("References")]
    [SerializeField] private Transform interactiveContent;
    [SerializeField] private TMP_Text text;

    public void OnSelected()
    {
        interactiveContent.DOKill();
        interactiveContent.rotation = Quaternion.identity;

        interactiveContent.DOPunchRotation(Vector3.forward * selectionPunchForce, selectionPunchDuration, 20, 1);
        interactiveContent.DOScale(hoverideScale, hoverideScaleTime);

        text.DOKill();
        text.DOColor(hoverColor, hoverideScaleTime);
    }

    public void OnDeselected()
    {
        interactiveContent.DOKill();
        interactiveContent.rotation = Quaternion.identity;

        interactiveContent.DOScale(1, deselectScaleTime);

        text.DOKill();
        text.DOColor(initColor, deselectScaleTime);
    }
}