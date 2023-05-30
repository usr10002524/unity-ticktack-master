using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトル表示制御クラス
/// </summary>
public class TitleController : MonoBehaviour
{
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
    }

    /// <summary>
    /// スタートボタンがクリックされたときの処理
    /// </summary>
    public void OnClickStart()
    {
        isTransition = true;
        SeManager.Instance.PlaySe(SeType.seDecide);
    }

    /// <summary>
    /// 遷移中かどうかチェックする。
    /// </summary>
    /// <returns>遷移中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsTransitionMainScene()
    {
        return isTransition;
    }
}
