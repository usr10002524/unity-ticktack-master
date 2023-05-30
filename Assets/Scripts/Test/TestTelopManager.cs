using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// TelopManagerテストスクリプト
/// </summary>
public class TestTelopManager : MonoBehaviour
{
    /// <summary>
    /// テロップタイプ
    /// </summary>
    [SerializeField] private TelopManager.TelopType telopType;

    /// <summary>
    /// StartTelop呼び出し
    /// </summary>
    [ContextMenu("TelopManager.StartTelop")]
    public void _TelopManagerStartTelop()
    {
        // Debug.LogFormat("_TelopManagerStartTelop() called.");
        TelopManager.Instance.StartTelop(telopType, FinishCallback);
    }

    /// <summary>
    /// テロップ終了コールバック
    /// </summary>
    private void FinishCallback()
    {
        Debug.LogFormat("TestTelopManager.FinishCallback() called.");
    }
}
