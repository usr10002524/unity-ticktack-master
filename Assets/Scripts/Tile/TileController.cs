using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイル制御クラス
/// </summary>
public class TileController : MonoBehaviour
{
    /// <summary>
    /// タイルの座標
    /// </summary>
    [SerializeField] private Vector2Int position;


    /// <summary>
    /// 位置をセットする
    /// </summary>
    /// <param name="pos">位置</param>
    public void SetPosition(Vector2Int pos)
    {
        position = pos;
    }

    /// <summary>
    /// 位置を取得する
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetPosition()
    {
        return position;
    }

    /// <summary>
    /// タイル座標に合わせて3D座標を更新する。
    /// </summary>
    public void UpdatePosition()
    {
        gameObject.transform.position = TileManager.Instance.GetTilePosition(position.x, position.y);
    }
}
