using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ローディング表示制御クラス
/// </summary>
public class LoadingController : MonoBehaviour
{
    /// <summary>
    /// text mesh pro
    /// </summary>
    [SerializeField] private TextMeshProUGUI textMeshPro;

    /// <summary>
    /// ローディング完了したか
    /// </summary>
    private bool isComplete;
    /// <summary>
    /// タイムアウトしたか
    /// </summary>
    private bool isTimeout;

    /// <summary>
    /// ローディング表示コルーチン
    /// </summary>
    private Coroutine loadingCoroutine;

    /// <summary>
    /// ロード中の文字列
    /// </summary>
    private static readonly string baseText = "Now Loading";
    /// <summary>
    /// ロード完了の文字列
    /// </summary>
    private static readonly string completeText = "COMPLETE!";


    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        isComplete = false;
        isTimeout = false;
    }

    /// <summary>
    /// ローディング表示を開始する。
    /// </summary>
    public void StartLoading()
    {
        StartLoadingCoroutine();
    }

    /// <summary>
    /// ロード完了状態へ移行する。
    /// </summary>
    public void SetComoplete()
    {
        // Debug.LogFormat("LoadingController.SetComoplete() called.");

        isComplete = true;
    }

    /// <summary>
    /// タイムアウト状態へ移行する。
    /// </summary>
    public void SetTimeout()
    {
        // Debug.LogFormat("LoadingController.SetTimeout() called.");

        isTimeout = true;
    }

    /// <summary>
    /// ローディング表示子ルーティンが終了したかチェックする。
    /// </summary>
    /// <returns>終了している場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsFinished()
    {
        return (loadingCoroutine == null);
    }

    /// <summary>
    /// ローディング表示コルーチンを起動する。
    /// </summary>
    private void StartLoadingCoroutine()
    {
        StopLoadingCoroutine();
        loadingCoroutine = StartCoroutine("LoadingCoroutine");
    }

    /// <summary>
    /// ローディング表示コルーチンを停止する。
    /// </summary>
    private void StopLoadingCoroutine()
    {
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }
    }

    /// <summary>
    /// ServerData をロードする
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator LoadingCoroutine()
    {
        // Debug.LogFormat("LoadingController.LoadingCoroutine() begin.");

        bool isEnd = false;
        float timer = 0.0f;
        float interval = 0.25f;
        int dotCount = 0;
        int maxDotCount = 3;
        float displayCompleteTime = 0.5f;
        float displayMinimumTimer = 0.0f;
        float minimumTime = 0.5f;

        while (!isEnd)
        {
            yield return null;

            displayMinimumTimer += Time.deltaTime;
            if (isComplete && (displayMinimumTimer >= minimumTime))
            {
                // Debug.LogFormat("LoadingController.LoadingCoroutine() isComplete:{0}.", isComplete);
                isEnd = true;
            }

            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0.0f;
                dotCount++;
                dotCount %= (maxDotCount + 1);
                SetText(dotCount);
            }

            if (isTimeout)
            {
                // Debug.LogFormat("LoadingController.LoadingCoroutine() isTimeout:{0}.", isTimeout);
                isEnd = true;
            }
        }

        if (isComplete)
        {
            SetCompleteText();
            yield return new WaitForSeconds(displayCompleteTime);
        }

        loadingCoroutine = null;
        // Debug.LogFormat("LoadingController.LoadingCoroutine() end.");
    }


    /// <summary>
    /// Loding + . を表示する
    /// </summary>
    /// <param name="dotCount"></param>
    private void SetText(int dotCount)
    {
        string text = baseText;
        for (int i = 0; i < dotCount; i++)
        {
            text += ".";
        }
        textMeshPro.SetText(text);
    }

    /// <summary>
    /// Complete を表示する
    /// </summary>
    private void SetCompleteText()
    {
        textMeshPro.SetText(completeText);
    }
}
