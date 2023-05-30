using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カラーイージング
/// </summary>
public class EaseImageColor : Easing
{
    /// <summary>
    /// 開始時の色
    /// </summary>
    [SerializeField] private Color startColor;
    /// <summary>
    /// 終了時の色
    /// </summary>
    [SerializeField] private Color endColor;

    /// <summary>
    /// イメージオブジェクト
    /// </summary>
    private Image image;
    /// <summary>
    /// オリジナルの色
    /// </summary>
    private Color origColor;

    /// <summary>
    /// イージングの更新
    /// </summary>
    /// <param name="t">進行割合</param>
    protected override void UpdateEasing(float t)
    {
        t = Mathf.Clamp01(t);
        Color color = Color.Lerp(startColor, endColor, t);

        if (image != null)
        {
            image.color = color;
        }
    }

    /// <summary>
    /// 初期化時の処理
    /// </summary>
    public override void OnInit()
    {
        if (easeObject != null)
        {
            image = easeObject.GetComponent<Image>();
            if (image != null)
            {
                origColor = image.color;
            }
        }

        if (origColor == null)
        {
            origColor = Color.black;
        }
    }

    /// <summary>
    /// イージングコルーチン起動時の処理
    /// </summary>
    protected override void OnEasingInit()
    {
        if (image != null)
        {
            image.color = origColor;
        }
    }
}
