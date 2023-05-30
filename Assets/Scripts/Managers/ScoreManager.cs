using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコア表示管理クラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// レベルに対するスコアレート
    /// </summary>
    [System.Serializable]
    public class ScoreRate
    {
        public int level;
        public int scoreRateX100;
    };

    /// <summary>
    /// スコア表示オブジェクトのプレファブ
    /// </summary>
    [SerializeField] private GameObject scorePrefab;
    /// <summary>
    /// スコア表示する際の親オブジェクト
    /// </summary>
    [SerializeField] private GameObject scoreBase;
    /// <summary>
    /// スコアレートが1秒間で減る量
    /// </summary>
    [SerializeField] private float decreaseRatePerSec;
    /// <summary>
    /// レベルごとのスコアレート
    /// </summary>
    [SerializeField] private List<ScoreRate> scoreRates;

    /// <summary>
    /// スコア表示オブジェクト
    /// </summary>
    private GameObject scoreObject;
    /// <summary>
    /// スコア表示制御クラス
    /// </summary>
    private ScoreController scoreController;

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    public static ScoreManager Instance { get; private set; }



    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        if (scorePrefab != null)
        {
            scoreObject = Instantiate(scorePrefab, scoreBase.transform);
        }
        if (scoreObject != null)
        {
            scoreController = scoreObject.GetComponent<ScoreController>();
        }
        if (scoreController != null)
        {
            scoreController.Initialize();
        }
    }

    /// <summary>
    /// ポーズ中かどうかをチェックする。
    /// </summary>
    /// <returns>ポーズ中の場合はtrueを、そうでない場合はfalseをかえす。</returns>
    public bool IsPause()
    {
        if (scoreController == null)
        {
            return false;
        }
        return scoreController.IsPause();
    }

    /// <summary>
    /// スコア表示をポーズ状態にする。
    /// ポーズ中はインクリメントが停止する。
    /// </summary>
    public void Pause()
    {
        if (scoreController == null)
        {
            return;
        }
        scoreController.Pause();
    }

    /// <summary>
    /// スコア表示のポーズを解除する
    /// </summary>
    public void Resume()
    {
        if (scoreController == null)
        {
            return;
        }
        scoreController.Resume();
    }

    /// <summary>
    /// スコアレートが1秒間に減る値を取得する
    /// </summary>
    /// <returns>スコアレートが1秒間に減る値</returns>
    public float GetDecreasRatePerSec()
    {
        return decreaseRatePerSec;
    }

    /// <summary>
    /// レベルに応じたスコアレートの100倍した値を取得する
    /// </summary>
    /// <param name="level">スコアレートを取得するレベル</param>
    /// <returns>スコアレートの100倍した値</returns>
    public int GetScoreRateX100(int level)
    {
        foreach (var item in scoreRates)
        {
            if (item.level == level)
            {
                return item.scoreRateX100;
            }
        }

        return 100;
    }
}
