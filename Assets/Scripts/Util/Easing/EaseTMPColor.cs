using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// カラーイージング
/// </summary>
public class EaseTMPColor : Easing
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
    /// TMPProオブジェクト
    /// </summary>
    private TextMeshProUGUI textMesh;

    /// <summary>
    /// イージングの更新
    /// </summary>
    /// <param name="t">進行割合</param>
    protected override void UpdateEasing(float t)
    {
        t = Mathf.Clamp01(t);
        Color color = Color.Lerp(startColor, endColor, t);

        if (textMesh != null)
        {
            textMesh.color = color;
        }
    }

    /// <summary>
    /// 初期化時の処理
    /// </summary>
    public override void OnInit()
    {
        if (easeObject != null)
        {
            textMesh = easeObject.GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// イージング開始時の処理
    /// </summary>
    protected override void OnEasingBegin()
    {
        if (textMesh != null)
        {
            textMesh.color = startColor;
        }
    }
}
