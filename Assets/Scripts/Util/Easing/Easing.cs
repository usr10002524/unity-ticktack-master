using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// イージング基底クラス
/// </summary>
public abstract class Easing : MonoBehaviour
{
    /// <summary>
    /// イージング対象オブジェクト
    /// </summary>
    [SerializeField] protected GameObject easeObject;
    /// <summary>
    /// アニメーションカーブ
    /// </summary>
    [SerializeField] protected AnimationCurve curve;
    /// <summary>
    /// イージング時間
    /// </summary>
    [SerializeField] protected float duration;
    /// <summary>
    /// 開始前に待機する時間
    /// </summary>
    [SerializeField] protected float startDelay;
    /// <summary>
    /// 終了後に待機する時間
    /// </summary>
    [SerializeField] protected float endDelay;

    /// <summary>
    /// 終了フラグ
    /// </summary>
    private bool isFinished;
    /// <summary>
    /// イージングコルーチン
    /// </summary>
    private Coroutine easingCroutine;

    /// <summary>
    /// イージング対象オブジェクトの設定
    /// </summary>
    /// <param name="gameObject">イージング対象オブジェクト</param>
    virtual public void SetGameObject(GameObject gameObject)
    {
        easeObject = gameObject;
    }

    /// <summary>
    /// 初期化時の処理
    /// </summary>
    public virtual void OnInit()
    {
    }

    /// <summary>
    /// イージングを開始する
    /// </summary>
    virtual public void StartEasing()
    {
        isFinished = false;
        if (easingCroutine != null)
        {
            StopCoroutine(easingCroutine);
            easingCroutine = null;
        }
        easingCroutine = StartCoroutine(EasingCoroutine());
    }

    /// <summary>
    /// イージングが終了したかチェックする
    /// </summary>
    /// <returns>終了した場合はtrue、そうでない場合はfalseを返す。</returns>
    virtual public bool IsFinished()
    {
        return isFinished;
    }

    /// <summary>
    /// イージング中かどうかチェックする
    /// </summary>
    /// <returns>イージング中はtrue、そうでない場合はfalseを返す。</returns>
    virtual public bool IsInEasing()
    {
        if (easingCroutine != null)
        {
            bool stopCoroutine = false;
            if (easeObject == null)
            {
                stopCoroutine = true;
            }
            else
            {
                if (!easeObject.activeInHierarchy)
                {
                    stopCoroutine = true;
                }
            }
            if (stopCoroutine)
            {
                StopCoroutine(easingCroutine);
                easingCroutine = null;
            }
        }
        return (easingCroutine != null);
    }

    /// <summary>
    /// イージングコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    virtual protected IEnumerator EasingCoroutine()
    {
        // easing 開始時の処理
        OnEasingInit();

        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }

        bool isEnd = false;
        float timer = 0.0f;

        // easing を実際に開始するときの処理
        OnEasingBegin();

        while (!isEnd)
        {
            yield return null;

            if (!easeObject.activeInHierarchy)
            {
                isEnd = true;
                continue;
            }

            timer += Time.deltaTime;
            float time = timer / duration;
            float t = curve.Evaluate(time);

            UpdateEasing(t);

            if (timer >= duration)
            {
                isEnd = true;
            }
        }

        // eaing が完了したときの処理
        OnEasingEnd();

        if (endDelay > 0)
        {
            yield return new WaitForSeconds(endDelay);
        }

        isFinished = true;
        easingCroutine = null;

        // easing 終了時の処理
        OnEasingFinish();
    }

    /// <summary>
    /// イージングの更新処理
    /// </summary>
    /// <param name="t">進行割合</param>
    protected abstract void UpdateEasing(float t);

    /// <summary>
    /// イージングコルーチン起動時の処理
    /// </summary>
    protected virtual void OnEasingInit() { }
    /// <summary>
    /// イージング開始時の処理
    /// </summary>
    protected virtual void OnEasingBegin() { }
    /// <summary>
    /// イージング終了時の処理
    /// </summary>
    protected virtual void OnEasingEnd() { }
    /// <summary>
    /// イージングコルーチン終了時の処理
    /// </summary>
    protected virtual void OnEasingFinish() { }
}
