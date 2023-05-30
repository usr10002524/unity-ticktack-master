using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 座標イージング
/// </summary>
public class EasePosition : Easing
{
    /// <summary>
    /// 開始地点
    /// </summary>
    [SerializeField] private Vector3 startPosition;
    /// <summary>
    /// 終了地点
    /// </summary>
    [SerializeField] private Vector3 endPosition;

    /// <summary>
    /// オリジナルの座標
    /// </summary>
    private Vector3 origPosition;

    /// <summary>
    /// イージングの更新処理
    /// </summary>
    /// <param name="t">進行割合</param>
    protected override void UpdateEasing(float t)
    {
        t = Mathf.Clamp01(t);
        Vector3 pos = Vector3.Lerp(startPosition, endPosition, t);

        if (easeObject != null)
        {
            easeObject.transform.position = pos;
        }
    }

    /// <summary>
    /// パラメータを設定する。
    /// </summary>
    /// <param name="startPos">開始位置</param>
    /// <param name="endPos">終了位置</param>
    public void SetParameter(Vector3 startPos, Vector3 endPos)
    {
        startPosition = startPos;
        endPosition = endPos;
    }
}
