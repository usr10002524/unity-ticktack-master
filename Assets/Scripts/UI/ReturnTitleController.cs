using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルへ戻るボタン制御クラス
/// </summary>
public class ReturnTitleController : MonoBehaviour
{
    /// <summary>
    /// ボタンのベースオブジェクト
    /// </summary>
    [SerializeField] GameObject buttonBase;

    /// <summary>
    /// 遷移中かどうか
    /// </summary>
    private bool isTransition;

    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        isTransition = false;
        ShowButton(false);
    }

    /// <summary>
    /// ボタンの表示状態を設定する。
    /// </summary>
    /// <param name="flag">trueの場合は表示、falseの場合は非表示</param>
    public void ShowButton(bool flag)
    {
        if (buttonBase == null)
        {
            return;
        }
        buttonBase.SetActive(flag);
    }

    /// <summary>
    /// 戻るボタンが押されたときの処理。
    /// </summary>
    public void OnClickReturn()
    {
        // Debug.LogFormat("ReturnTitleController.OnClickReturn() called.");
        SeManager.Instance.PlaySe(SeType.seDecide);
        isTransition = true;
    }

    /// <summary>
    /// 遷移中かどうかをチェッくする。
    /// </summary>
    /// <returns>遷移中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsTransitionTitleScene()
    {
        return isTransition;
    }
}
