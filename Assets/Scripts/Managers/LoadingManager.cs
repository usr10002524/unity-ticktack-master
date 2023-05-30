using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ローディング管理クラス
/// </summary>
public class LoadingManager : MonoBehaviour
{
    /// <summary>
    /// ローディングゲームオブジェクトのプレファブ
    /// </summary>
    [SerializeField] private GameObject loadingPrefab;
    /// <summary>
    /// ローディングゲームオブジェクトを生成する際の親オブジェクト
    /// </summary>
    [SerializeField] private GameObject loadingBase;


    /// <summary>
    /// ローディングゲームオブジェクト
    /// </summary>
    private GameObject loadingObject;
    /// <summary>
    /// ローディングクラス
    /// </summary>
    private LoadingController loadingController;

    /// <summary>
    /// シングルトンのインスタンス。
    /// </summary>
    public static LoadingManager Instance { get; private set; }

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
        if (loadingPrefab == null)
        {
            return;
        }
        if (loadingBase == null)
        {
            return;
        }

        loadingObject = Instantiate(loadingPrefab, loadingBase.transform);
        if (loadingObject == null)
        {
            return;
        }
        loadingController = loadingObject.GetComponent<LoadingController>();
        if (loadingController == null)
        {
            return;
        }

        loadingController.Initialize();
    }

    /// <summary>
    /// ローディング表示を開始する。
    /// </summary>
    public void StartLoading()
    {
        if (loadingController == null)
        {
            return;
        }
        loadingController.StartLoading();
    }

    /// <summary>
    /// ロード完了表示に切り替える。
    /// </summary>
    public void SetComplete()
    {
        if (loadingController == null)
        {
            return;
        }
        loadingController.SetComoplete();
    }

    /// <summary>
    /// ロードタイムアウト表示に切り替える。
    /// </summary>
    public void SetTimeout()
    {
        if (loadingController == null)
        {
            return;
        }
        loadingController.SetTimeout();
    }

    /// <summary>
    /// ローディング表示が完了したかチェックする
    /// </summary>
    /// <returns>ローディング表示が完了した場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsFinished()
    {
        if (loadingController == null)
        {
            return true;
        }
        return loadingController.IsFinished();
    }
}
