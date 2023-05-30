using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// スコア表示制御クラス
/// </summary>
public class ScoreController : MonoBehaviour
{
    /// <summary>
    /// スコア表示用
    /// </summary>
    [SerializeField] private TextMeshProUGUI scoreText;
    /// <summary>
    /// レートレベル表示用
    /// </summary>
    [SerializeField] private TextMeshProUGUI rateLevelText;
    /// <summary>
    /// レート表示用
    /// </summary>
    [SerializeField] private TextMeshProUGUI rateText;
    /// <summary>
    /// スライダー
    /// </summary>
    [SerializeField] private Slider rateSlider;

    /// <summary>
    /// 表示用スコア
    /// </summary>
    private int drawScore;
    /// <summary>
    /// 実際のスコア
    /// </summary>
    private int actualScore;

    /// <summary>
    /// 1秒間に現象するスコアレートの値
    /// </summary>
    private float decreaseRatePerSec;
    /// <summary>
    /// 表示用スコアレート
    /// </summary>
    private float drawRate;
    /// <summary>
    /// 実際のスコアレート
    /// </summary>
    private float actualRate;

    /// <summary>
    /// 表示用レートレベル
    /// </summary>
    private int drawRateLevel;
    /// <summary>
    /// 実際のレートレベル
    /// </summary>
    private int actualRateLevel;

    /// <summary>
    /// インクリメント用コルーチン
    /// </summary>
    private Coroutine incrementCoroutine;
    /// <summary>
    /// レート用コルーチン
    /// </summary>
    private Coroutine rateCoroutine;

    /// <summary>
    /// ポーズ中かどうか
    /// </summary>
    private bool isPause;


    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        actualScore = PlayerManager.Instance.GetScore();
        actualRate = PlayerManager.Instance.GetClampRate();
        actualRateLevel = PlayerManager.Instance.GetRateLevel();
        decreaseRatePerSec = ScoreManager.Instance.GetDecreasRatePerSec();
        isPause = true;

        SyncScore();
        SyncRateLevel();

        StartIncrementCoroutine();
        StartRateCoroutine();
    }

    /// <summary>
    /// 表示用スコアに実際のスコアを同期させる。
    /// </summary>
    public void SyncScore()
    {
        drawScore = actualScore;
        DrawScore();
    }

    /// <summary>
    /// 表示用レートに実際のレートを同期させる。
    /// </summary>
    public void SyncRateLevel()
    {
        drawRate = actualRate;
        drawRateLevel = actualRateLevel;
        DrawRateLevel();
    }

    /// <summary>
    /// スコアインクリメントコルーチンを起動する。
    /// </summary>
    public void StartIncrementCoroutine()
    {
        StopIncrementCoroutine();

        incrementCoroutine = StartCoroutine("IncrementCoroutine");
    }

    /// <summary>
    /// スコアインクリメントコルーチンを停止する。
    /// </summary>
    public void StopIncrementCoroutine()
    {
        if (incrementCoroutine != null)
        {
            StopCoroutine(incrementCoroutine);
            incrementCoroutine = null;
        }
    }

    /// <summary>
    /// レート表示コルーチンを起動する。
    /// </summary>
    public void StartRateCoroutine()
    {
        StopRateCroutine();
        rateCoroutine = StartCoroutine("RateLevelCoroutine");
    }

    /// <summary>
    /// レート表示コルーチンを停止する。
    /// </summary>
    public void StopRateCroutine()
    {
        if (rateCoroutine != null)
        {
            StopCoroutine(rateCoroutine);
            rateCoroutine = null;
        }
    }

    /// <summary>
    /// ポーズ中かどうかをチェックする。
    /// </summary>
    /// <returns>ポーズ中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsPause()
    {
        return isPause;
    }

    /// <summary>
    /// ポーズ状態にする。
    /// </summary>
    public void Pause()
    {
        isPause = true;
    }

    /// <summary>
    /// ポーズ状態を解除する。
    /// </summary>
    public void Resume()
    {
        isPause = false;
    }


    /// <summary>
    /// スコアを表示する。
    /// </summary>
    private void DrawScore()
    {
        scoreText.text = string.Format("{0}", drawScore);
    }

    /// <summary>
    /// レートレベルを表示する。
    /// </summary>
    private void DrawRateLevel()
    {
        rateLevelText.text = string.Format("Lv. {0}", drawRateLevel);
        rateSlider.value = drawRate;

        int rateX100 = ScoreManager.Instance.GetScoreRateX100(drawRateLevel);
        int decimalNumber = rateX100 / 100;
        int pointNumber = (rateX100 - (decimalNumber * 100)) / 10;
        rateText.text = string.Format("x {0}.{1,1}", decimalNumber, pointNumber);
    }

    /// <summary>
    /// インクリメントコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator IncrementCoroutine()
    {
        bool isEnd = false;

        while (!isEnd)
        {
            yield return null;
            actualScore = PlayerManager.Instance.GetScore();

            int diff = actualScore - drawScore;
            if (diff < 0)
            {
                // スコアが表示より減っていれば即時同期
                SyncScore();
                diff = 0;
            }

            if (diff == 0)
            {
                continue;   // 差異がなければ何もしない
            }

            int add = Mathf.Max(diff / 100, 1);
            drawScore += add;
            if (drawScore > actualScore)
            {
                drawScore = actualScore;
            }
            DrawScore();
        }
    }

    /// <summary>
    /// レート表示コルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator RateLevelCoroutine()
    {
        bool isEnd = false;

        while (!isEnd)
        {
            yield return null;

            if (IsPause())
            {
                continue;
            }

            float dcreaseRate = decreaseRatePerSec * Time.deltaTime;
            PlayerManager.Instance.AddRate(-dcreaseRate);
            actualRate = PlayerManager.Instance.GetClampRate();
            actualRateLevel = PlayerManager.Instance.GetRateLevel();

            SyncRateLevel();
            DrawScore();
        }
    }


}
