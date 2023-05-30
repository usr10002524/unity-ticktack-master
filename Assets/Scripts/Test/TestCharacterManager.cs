using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterManagerテストスクリプト
/// </summary>
public class TestCharacterManager : MonoBehaviour
{
    /// <summary>
    /// スピード加算値
    /// </summary>
    [SerializeField] float addSpeed = 0.0f;
    /// <summary>
    /// ボイスのディレイ値
    /// </summary>
    [SerializeField] float voiceDealy = 0.0f;

    /// <summary>
    /// 初期化呼び出し
    /// </summary>
    [ContextMenu("CharacterManager.Initialize")]
    public void _CharacterManagerInitialize()
    {
        CharacterManager.Instance.Initialize();
    }

    /// <summary>
    /// Run呼び出し
    /// </summary>
    [ContextMenu("CharacterManager.Run")]
    public void _CharacterManagerRun()
    {
        CharacterManager.Instance.Run();
    }

    /// <summary>
    /// Stop呼び出し
    /// </summary>
    [ContextMenu("CharacterManager.Stop")]
    public void _CharacterManagerStop()
    {
        CharacterManager.Instance.Stop();
    }

    /// <summary>
    /// Jump呼び出し
    /// </summary>
    [ContextMenu("CharacterManager.Jump")]
    public void _CharacterManagerJump()
    {
        CharacterManager.Instance.Jump();
    }

    /// <summary>
    /// AddSpeed呼び出し
    /// </summary>
    [ContextMenu("CharacterManager.AddSpeed")]
    public void _CharacterManagerAddSpeed()
    {
        CharacterManager.Instance.AddSpeed(addSpeed);
    }

    /// <summary>
    /// GameStart呼び出し
    /// </summary>
    [ContextMenu("CharacterManager.GameStart")]
    public void _CharacterManagerGameStart()
    {
        CharacterManager.Instance.GameStart(voiceDealy);
    }

    /// <summary>
    /// GameOver呼び出し
    /// </summary>
    [ContextMenu("CharacterManager.GameOver")]
    public void _CharacterManagerGameOver()
    {
        CharacterManager.Instance.GameOver(voiceDealy);
    }

    /// <summary>
    /// ScoreGet呼び出し
    /// </summary>
    [ContextMenu("CharacterManager.ScoreGet")]
    public void _CharacterManagerScoreGet()
    {
        CharacterManager.Instance.ScoreGet(voiceDealy);
    }
}
