using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2D座標イージング
/// </summary>
public class EasePosition2D : Easing
{
    /// <summary>
    /// 開始地点
    /// </summary>
    [SerializeField] private Vector2 startPosition;
    /// <summary>
    /// 終了地点
    /// </summary>
    [SerializeField] private Vector2 endPosition;

    /// <summary>
    /// RectTransform
    /// </summary>
    private RectTransform rectTransform;

    protected override void UpdateEasing(float t)
    {
        t = Mathf.Clamp01(t);
        Vector2 pos = Vector2.Lerp(startPosition, endPosition, t);

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = pos;
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
            rectTransform.anchoredPosition = startPosition;
        }
    }
}
