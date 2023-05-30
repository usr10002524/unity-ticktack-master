using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fadeの各種定数
/// </summary>
public class FadeConst
{
    /// <summary>
    /// フェード時間
    /// </summary>
    public static readonly float sceneTransitDuration = 0.5f;
    /// <summary>
    /// フェードの色
    /// </summary>
    public static readonly Color sceneTransitColor = Color.black;
    /// <summary>
    /// フェードのソート順
    /// </summary>
    public static readonly int sceneTransitSortOrder = 10;
    /// <summary>
    /// フェードスピード係数
    /// </summary>
    public static readonly float fadeDump = 1.0f;
}
