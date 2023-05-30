using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpawnManagerテストスクリプト
/// </summary>
public class TestSpawnManager : MonoBehaviour
{
    /// <summary>
    /// Initialize呼び出し
    /// </summary>
    [ContextMenu("SpawnManager.Initialize")]
    public void _SpawnManagerInitialize()
    {
        SpawnManager.Instance.Initialize();
    }
}
