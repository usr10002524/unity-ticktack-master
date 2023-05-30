using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InstManagerのテストスクリプト
/// </summary>
public class TestInstManager : MonoBehaviour
{
    /// <summary>
    /// Initialize呼び出し
    /// </summary>
    [ContextMenu("InstManager.Initialize")]
    public void _InstManagerInitialize()
    {
        InstManager.Instance.Initialize();
    }

    /// <summary>
    /// OnToggle呼び出し
    /// </summary>
    [ContextMenu("InstManager.OnToggle")]
    public void _InstManagerOnToggle()
    {
        InstManager.Instance.OnToggle();
    }

    /// <summary>
    /// OnClear呼び出し
    /// </summary>
    [ContextMenu("InstManager.OnClose")]
    public void _InstManagerOnClose()
    {
        InstManager.Instance.OnClose();
    }

    /// <summary>
    /// OnNext呼び出し
    /// </summary>
    [ContextMenu("InstManager.OnNext")]
    public void _InstManagerOnNext()
    {
        InstManager.Instance.OnNext();
    }

    /// <summary>
    /// OnPrev呼び出し
    /// </summary>
    [ContextMenu("InstManager.OnPrev")]
    public void _InstManagerOnPrev()
    {
        InstManager.Instance.OnPrev();
    }
}
