using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルに戻るボタン表示管理クラス
/// </summary>
public class ReturnTitleManager : MonoBehaviour
{
    /// <summary>
    /// ボタンゲームオブジェクトのプレファブ
    /// </summary>
    [SerializeField] private GameObject returnTitlePerfab;
    /// <summary>
    /// ボタンゲームオブジェクトの親オブジェクト
    /// </summary>
    [SerializeField] private GameObject returnTitleBase;

    /// <summary>
    /// ボタンゲームオブジェクト
    /// </summary>
    private GameObject returnTitleObject;
    /// <summary>
    /// ボタン制御クラス
    /// </summary>
    private ReturnTitleController returnTitleController;

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    /// <value></value>
    public static ReturnTitleManager Instance { get; private set; }


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
        if (returnTitlePerfab == null)
        {
            return;
        }
        if (returnTitleBase == null)
        {
            return;
        }

        returnTitleObject = Instantiate(returnTitlePerfab, returnTitleBase.transform);
        if (returnTitleObject == null)
        {
            return;
        }

        returnTitleController = returnTitleObject.GetComponent<ReturnTitleController>();
        if (returnTitleController == null)
        {
            return;
        }

        returnTitleController.Initialize();
        ShowButton(false);
    }

    /// <summary>
    /// ボタンの表示、非表示を行う。
    /// </summary>
    /// <param name="flag">表示フラグ</param>
    public void ShowButton(bool flag)
    {
        if (returnTitleController == null)
        {
            return;
        }
        returnTitleController.ShowButton(flag);
    }

    /// <summary>
    /// タイトルに遷移中かどうかを確認する。
    /// </summary>
    /// <returns>遷移中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsTransitionTitleScene()
    {
        if (returnTitleController == null)
        {
            return true;
        }
        return returnTitleController.IsTransitionTitleScene();
    }
}
