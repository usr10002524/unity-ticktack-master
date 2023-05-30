using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TileManagerテストスクリプト
/// </summary>
public class TestTileManager : MonoBehaviour
{
    /// <summary>
    /// Initialize呼び出し
    /// </summary>
    [ContextMenu("TileManager.Initialize")]
    public void _TilemanagerInitialize()
    {
        TileManager.Instance.Initialize();
    }

    /// <summary>
    /// 横位置
    /// </summary>
    [SerializeField]
    [Range(0, 3)]
    private int col;
    /// <summary>
    /// 縦位置
    /// </summary>
    [SerializeField]
    [Range(0, 3)]
    private int row;

    /// <summary>
    /// Slide呼び出し
    /// </summary>
    [ContextMenu("TileManager.Slide")]
    public void _TilemanagerSlide()
    {
        TileManager.Instance.Slide(col, row);
        TileManager.Instance.UpdateTilemap();
        TileManager.Instance.UpdateConnection();
    }
}
