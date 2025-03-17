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

    [Space(10)]
    [SerializeField] MeshFilter _fillFilter;
    [SerializeField] MeshRenderer _fillRenderer;

    [SerializeField] MeshFilter _outlineFilter;
    [SerializeField] MeshRenderer _outlineRenderer;

    Queue<BlobCloneVisual> clones = new();

    struct BlobCloneVisual
    {
        public GameObject content;
        public MeshFilter fillFilter, outlineFilter;
        public MeshRenderer fillRenderer, outlineRenderer;

        public Material fillMat, outlineMat;
        
        public BlobCloneVisual(GameObject clone, MeshFilter fillFilter, MeshRenderer fillRenderer, MeshFilter outlineFilter, MeshRenderer outlineRenderer)
        {
            this.content = clone;

            this.fillFilter = fillFilter;
            this.fillRenderer = fillRenderer;

            this.outlineFilter = outlineFilter;
            this.outlineRenderer = outlineRenderer;

            fillMat = new Material(fillRenderer.material);
            outlineMat = new Material(outlineRenderer.material);

            this.fillRenderer.material = fillMat;
            this.outlineRenderer.material = outlineMat;
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
        for (int i = 0; i < cloneCount; i++)
        {
            BlobCloneVisual clone = GetClone();
            clone.content.gameObject.SetActive(true);

            //clone.fillFilter.mesh = _fillFilter.sharedMesh;
            //clone.outlineFilter.mesh = _outlineFilter.sharedMesh;

            //clone.fillrenderer.material.color =
            //    new color(_fillrenderer.material.color.r, _fillrenderer.material.color.g, _fillrenderer.material.color.b, clonestartingalpha);
            //clone.outlinerenderer.material.color =
            //    new color(_outlinerenderer.material.color.r, _outlinerenderer.material.color.g, _outlinerenderer.material.color.b, clonestartingalpha);

            clone.fillMat.DOFade(0, cloneLifeTime);
            clone.outlineMat.DOFade(0, cloneLifeTime).OnComplete(() =>
            {
                clone.content.gameObject.SetActive(false);
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
        GameObject clone = new GameObject("BlobClone");
        clone.transform.SetParent(transform);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(clone.transform);
        MeshFilter fillFilter = fill.AddComponent<MeshFilter>();
        MeshRenderer fillRenderer = fill.AddComponent<MeshRenderer>();

        GameObject outline = new GameObject("Outline");
        outline.transform.SetParent(clone.transform);
        MeshFilter outlineFilter = outline.AddComponent<MeshFilter>();
        MeshRenderer outlineRenderer = outline.AddComponent<MeshRenderer>();

        BlobCloneVisual blobCloneVisual = new BlobCloneVisual(clone, fillFilter, fillRenderer, outlineFilter, outlineRenderer);
        clone.SetActive(false);

        clones.Enqueue(blobCloneVisual);
    }
}