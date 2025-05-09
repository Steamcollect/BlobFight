using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;

public static class Utils
{
    #region QUATERNION
    public static Quaternion QuaternionSmoothDamp(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
    {
        if (Time.deltaTime == 0) return current;
        if (smoothTime == 0) return target;

        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
          Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
          Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
          Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
        );
    }
    #endregion

    #region IENUMERABLE
    public static T GetRandom<T>(this IEnumerable<T> elems)
    {
        if (elems.Count() == 0)
        {
            Debug.LogError("Try to get random elem from empty IEnumerable");
        }
        return elems.ElementAt(new System.Random().Next(0, elems.Count()));
    }
    #endregion

    #region COROUTINE
    public static IEnumerator Delay(float delay, Action ev)
    {
        yield return new WaitForSeconds(delay);
        ev?.Invoke();
    }
    #endregion

    #region SCENE
    /// <summary>
    /// Load Scene Asyncronely and call action at the end
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <param name="loadMode"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator LoadSceneAsync(int sceneIndex, LoadSceneMode loadMode, Action action = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, loadMode);

        yield return new WaitUntil(() => asyncLoad.isDone);

        action?.Invoke();
    }
    
    /// <summary>
    /// Load Scene Asyncronely and call action at the end
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <param name="loadMode"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadMode, Action action = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadMode);

        yield return new WaitUntil(() => asyncLoad.isDone);

        action?.Invoke();
    }

    /// <summary>
    /// Unload Scene Asyncronely and call action at the end
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator UnloadSceneAsync(string sceneName, Action action = null)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);

        yield return new WaitUntil(() => asyncLoad.isDone);

        action?.Invoke();
    }
    public static IEnumerator UnloadSceneAsync(int sceneIndex, Action action = null)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneIndex);

        yield return new WaitUntil(() => asyncLoad.isDone);

        action?.Invoke();
    }
    #endregion

    #region DOTWEEN
    public static void BumpVisual(this Transform t)
    {
        t.DOKill();
        t.DOScale(1.1f, .06f).OnComplete(() =>
        {
            t.DOScale(1, .08f);
        });
    }
    public static void BumpVisual(this Transform t, float targetScale, Action OnComplecte)
    {
        t.DOKill();
        t.DOScale(targetScale * 1.1f, .06f).OnComplete(() =>
        {
            t.DOScale(targetScale, .08f).OnComplete(() => { OnComplecte.Invoke(); });
        });
    }
    #endregion

    # region BLOB
    public static void GetPixelColor()
    {

    }
    #endregion
}