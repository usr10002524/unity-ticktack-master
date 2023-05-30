using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LoadingManagerテストスクリプト
/// </summary>
public class TestLoadingManager : MonoBehaviour
{
    /// <summary>
    /// Initialize呼び出し
    /// </summary>
    [ContextMenu("LoadingManager.Initialize")]
    public void _LoadingManagerInitialize()
    {
        LoadingManager.Instance.Initialize();
    }

    /// <summary>
    /// StartLoading呼び出し
    /// </summary>
    [ContextMenu("LoadingManager.StartLoading")]
    public void _LoadingManagerStartLoading()
    {
        LoadingManager.Instance.StartLoading();
    }

    /// <summary>
    /// SetComplete呼び出し
    /// </summary>
    [ContextMenu("LoadingManager.SetComplete")]
    public void _LoadingManagerSetComplete()
    {
        LoadingManager.Instance.SetComplete();
    }

    /// <summary>
    /// SetTimeout呼び出し
    /// </summary>
    [ContextMenu("LoadingManager.SetTimeout")]
    public void _LoadingManagerSetTimeout()
    {
        LoadingManager.Instance.SetTimeout();
    }
}
