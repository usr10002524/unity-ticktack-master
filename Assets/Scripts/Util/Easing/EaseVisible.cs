using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示イージング
/// </summary>
public class EaseVisible : Easing
{
    /// <summary>
    /// OnInit時にアクティブ化するか
    /// </summary>
    [SerializeField] private bool activateOnInit = false;
    /// <summary>
    /// OnBegin時にアクティブ化するか
    /// </summary>
    [SerializeField] private bool activateOnBegin = true;
    /// <summary>
    /// OnEnd時に非アクティブ化するか
    /// </summary>
    [SerializeField] private bool inActivateOnEnd = true;
    /// <summary>
    /// OnFinish時に非アクティブ化するか
    /// </summary>
    [SerializeField] private bool inActivateOnFinish = false;

    /// <summary>
    /// イージングの更新処理
    /// </summary>
    /// <param name="t">進行割合</param>
    protected override void UpdateEasing(float t)
    {

    }

    /// <summary>
    /// イージングコルーチン起動時の処理
    /// </summary>
    protected override void OnEasingInit()
    {
        if (activateOnInit)
        {
            easeObject.SetActive(true);
        }
    }
    /// <summary>
    /// イージング開始時の処理
    /// </summary>
    protected override void OnEasingBegin()
    {
        if (activateOnBegin)
        {
            easeObject.SetActive(true);
        }
    }
    /// <summary>
    /// イージング終了時の処理
    /// </summary>
    protected override void OnEasingEnd()
    {
        if (inActivateOnEnd)
        {
            easeObject.SetActive(false);
        }
    }
    /// <summary>
    /// イージングコルーチン終了時の処理
    /// </summary>
    protected override void OnEasingFinish()
    {
        if (inActivateOnFinish)
        {
            easeObject.SetActive(false);
        }
    }
}
