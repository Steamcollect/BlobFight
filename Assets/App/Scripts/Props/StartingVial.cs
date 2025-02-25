using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StartingVial : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int breakRequire;
    int breakCount;

    bool canBreak = true;

    //[Header("References")]
    BlobMotor blob;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public void Setup(BlobMotor blob)
    {
        this.blob = blob;
        blob.GetInput().breakVialInput += OnPress;
        OnPress();
    }

    void OnPress()
    {
        if (!canBreak) return;

        breakCount++;
        if(breakCount >= breakRequire)
        {
            blob.Spawn(transform.position);
            canBreak = false;
            gameObject.SetActive(false);
            return;
        }

        transform.DOPunchRotation(Vector3.forward * 8, .5f, 20, 1);
        StartCoroutine(BreakCooldown());
    }

    IEnumerator BreakCooldown()
    {
        canBreak = false;
        yield return new WaitForSeconds(.6f);
        canBreak = true;
    }
}