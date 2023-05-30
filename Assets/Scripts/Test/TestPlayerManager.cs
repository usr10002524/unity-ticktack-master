using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerManagerテストスクリプト
/// </summary>
public class TestPlayerManager : MonoBehaviour
{
    /// <summary>
    /// スコア加算値
    /// </summary>
    [SerializeField] private int addScore = 0;
    /// <summary>
    /// レート加算値
    /// </summary>
    [SerializeField] private float addRate = 0.0f;

    /// <summary>
    /// AddScore呼び出し
    /// </summary>
    [ContextMenu("PlayerManager.AddScore")]
    public void _PlayerManagerAddScore()
    {
        PlayerManager.Instance.AddScore(addScore);
    }

    /// <summary>
    /// AddRate呼び出し
    /// </summary>
    [ContextMenu("PlayerManager.AddRate")]
    public void _PlayerManagerAddRate()
    {
        PlayerManager.Instance.AddRate(addRate);
    }
}
