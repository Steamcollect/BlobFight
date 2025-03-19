using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobDashVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int cloneCount;
    [SerializeField] float timeBetweenClone;

    [SerializeField] float cloneStartingAlpha;
    [SerializeField] float cloneLifeTime;

    [Space(10)]
    [SerializeField] int cloneCountToInstantiate = 10;

    [Header("References")]
    [SerializeField] BlobDash blobDash;
    [SerializeField] BlobVisual visual;

    Queue<BlobCloneVisual> clones = new();

    struct BlobCloneVisual
    {
        public SpriteRenderer graphics;

        public BlobCloneVisual(SpriteRenderer graphics)
        {
            this.graphics = graphics;
        }
    }

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnEnable()
    {
        blobDash.OnDash += OnDash;
    }
    private void OnDisable()
    {
        blobDash.OnDash -= OnDash;
    }

    private void Start()
    {
        for (int i = 0; i < cloneCountToInstantiate; i++)
        {
            CreateClone();
        }
    }

    void OnDash()
    {
        StartCoroutine(DashVisual());
    }
    IEnumerator DashVisual()
    {
        yield return new WaitForSeconds(.05f);

        for (int i = 0; i < cloneCount; i++)
        {
            BlobCloneVisual clone = GetClone();

            clone.graphics.sprite = visual.GetGraphics().sprite;
            clone.graphics.color = visual.GetGraphics().color;

            clone.graphics.transform.position = visual.transform.position;
            clone.graphics.transform.localScale = Vector2.Scale(visual.transform.localScale, visual.transform.parent.localScale);
            clone.graphics.transform.rotation = visual.transform.rotation;

            clone.graphics.gameObject.SetActive(true);

            clone.graphics.DOFade(0, cloneLifeTime).OnComplete(() =>
            {
                clone.graphics.gameObject.SetActive(false);
                clones.Enqueue(clone);
            });

            yield return new WaitForSeconds(timeBetweenClone);
        }
    }

    BlobCloneVisual GetClone()
    {
        if (clones.Count <= 0) CreateClone();
        return clones.Dequeue();
    }
    void CreateClone()
    {
        GameObject clone = new GameObject("Blob Dash Clone");
        clone.transform.SetParent(transform);
        SpriteRenderer graphics = clone.AddComponent<SpriteRenderer>();

        BlobCloneVisual blobCloneVisual = new BlobCloneVisual(graphics);
        clone.SetActive(false);

        clones.Enqueue(blobCloneVisual);
    }
}