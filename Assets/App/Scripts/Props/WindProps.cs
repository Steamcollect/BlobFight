using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[Serializable]
//public struct RbDelay
//{
//    public Rigidbody2D rb2D;
//    public float delay;
//    public bool canActiveForce;
//}
public class WindProps : GameProps
{
    [Header("Settings")]
    [SerializeField] float windForce;
    //[SerializeField] float windForceDelay;
    bool isLaunched = false;

    //[Header("References")]
    List<Rigidbody2D> rbs = new();
    //List<RbDelay> rbDelays = new();
    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public override void Launch()
    {
        isLaunched = true;
    }

    private void FixedUpdate()
    {
        if (!isLaunched) return;

        if(rbs.Count > 0)
        {
            //StartCoroutine(DelayAddForce());
            foreach (var rbs in rbs)
            {
                rbs.AddForce(transform.up * windForce);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Rigidbody2D rb))
        {
            //RbDelay rbDelay = new();
            //rbDelay.rb2D = rb;
            //rbDelay.delay = windForceDelay;
            //rbDelay.canActiveForce = false;
            //rbDelays.Add(rbDelay);
            rbs.Add(rb);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            rbs.Remove(rb);
        }
    }
    //IEnumerator DelayAddForce()
    //{
    //    yield return new WaitForSeconds(windForceDelay);
    //}
    public void SetWindForce(float windForceChange)
    {
        windForce = windForceChange;
    }

    public float GetWindForce()
    {
        return windForce;
    }
}