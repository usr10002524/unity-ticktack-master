using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームバージョン
/// </summary>
public class GameVersion : MonoBehaviour
{
    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        Debug.LogFormat("{0} version:{1}", Application.productName, Application.version);
    }
}
