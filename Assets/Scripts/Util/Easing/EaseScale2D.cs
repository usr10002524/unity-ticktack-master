using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2Dスケールイージング
/// </summary>
public class EaseScale2D : Easing
{
    /// <summary>
    /// 開始スケーリング
    /// </summary>
    [SerializeField] private Vector2 startScale;
    /// <summary>
    /// 終了スケーリング
    /// </summary>
    [SerializeField] private Vector2 endScale;

    /// <summary>
    /// RectTransform
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// イージングの更新処理
    /// </summary>
    /// <param name="t">進行割合</param>
    protected override void UpdateEasing(float t)
    {
        t = Mathf.Clamp01(t);
        Vector2 scale = Vector2.Lerp(startScale, endScale, t);

        if (rectTransform != null)
        {
            rectTransform.localScale = scale;
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void OnInit()
    {
        if (easeObject != null)
        {
            rectTransform = easeObject.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// イージング開始時の処理
    /// </summary>
    protected override void OnEasingBegin()
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = startScale;
        }
    }
}
