using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コア関連
/// </summary>
namespace Core
{
    /// <summary>
    /// タイルの種類
    /// </summary>
    public enum Tile
    {
        None,

        StraightH,
        Cross,

        Curve01,
        Curve02,

        DoubleCurve01,
        DoubleCurve02,

        StraightV,
        Curve03,
        Curve04,
    }

    /// <summary>
    /// タイルの辺
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// なし
        /// </summary>
        None,
        /// <summary>
        /// 上側
        /// </summary>
        Up,
        /// <summary>
        /// 下側
        /// </summary>
        Down,
        /// <summary>
        /// 左側
        /// </summary>
        Left,
        /// <summary>
        /// 右側
        /// </summary>
        Right,
    }

    /// <summary>
    /// パスの向き
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// なし
        /// </summary>
        None,

        /// <summary>
        /// 順方向
        /// </summary>
        Normal,
        /// <summary>
        /// 逆方向
        /// </summary>
        Reverse,
    }
#if false
    /// <summary>
    /// スポーン出現場所
    /// </summary>
    public enum SpawnArea
    {
        /// <summary>
        /// なし
        /// </summary>
        None,
        /// <summary>
        /// 真ん中
        /// </summary>
        Center,
        /// <summary>
        /// 左上
        /// </summary>
        TopLeft,
        /// <summary>
        /// 左下
        /// </summary>
        BottomLeft,
        /// <summary>
        /// 右上
        /// </summary>
        TopRight,
        /// <summary>
        /// 右下
        /// </summary>
        BottomRgiht,
    }
#endif

    /// <summary>
    /// スポーンエリア
    /// </summary>
    public enum SpawnPatternArea
    {
        // なし
        None,

        // 真ん中
        C,
        // 上
        U,
        // 下
        D,
        // 左
        L,
        // 右
        R,

        // 左上
        UL,
        // 右上
        UR,
        // 左下
        DL,
        // 右下
        DR,
    }

    /// <summary>
    /// スポーンパターン
    /// </summary>
    public enum SpawnPattern
    {
        /// <summary>
        /// なし
        /// </summary>
        None,

        // 1個パターン
        // 中央
        C,
        // 左上
        UL,
        // 右上
        UR,
        // 左下
        DL,
        // 右下
        DR,

        // 2個パターン
        // 中央横2つ
        H2,
        // 中央縦2つ
        V2,
        // 左上2つ
        UL2,
        // 右上2つ
        UR2,
        // 左下2つ
        DL2,
        // 右下2つ
        DR2,

        // 3個パターン
        // 中央横3つ
        H3,
        // 中央縦3つ
        V3,

        // 左上3つ
        UL3,
        // 右上3つ
        UR3,
        // 左下3つ
        DL3,
        // 右下3つ
        DR3,
    }

    // スポーン対象
    public enum SpawnType
    {
        // なし
        None,
        // スコア小
        ScoreSamll,
        // スコア中
        ScoreMedium,
        // スコア大
        ScoreLarge,
        // スピードアップ
        SpeedUp,
        // スピードダウン
        SpeedDown,
    }

    // 各種定数
    public class Consts
    {
        // タイルの横幅
        public static readonly float TileWidth = 5.0f;
        // タイルの縦幅
        public static readonly float TileHeight = 5.0f;

    }

    /// <summary>
    /// ユーティリティ
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// 指定した座標に対する上下左右に隣接する座標を算出する。
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <param name="side">隣接する辺</param>
        /// <returns>隣接するタイルの座標を返す</returns>
        public static Vector2Int Offset(Vector2Int pos, Side side)
        {
            switch (side)
            {
                case Side.Up: return (pos - Vector2Int.up);
                case Side.Down: return (pos - Vector2Int.down);
                case Side.Left: return (pos + Vector2Int.left);
                case Side.Right: return (pos + Vector2Int.right);
                default: return pos;
            }
        }

        /// <summary>
        /// 指定した辺に対向する辺を取得する。
        /// </summary>
        /// <param name="side">辺</param>
        /// <returns>対抗する辺</returns>
        public static Side Opposite(Side side)
        {
            switch (side)
            {
                case Side.Up: return Side.Down;
                case Side.Down: return Side.Up;
                case Side.Left: return Side.Right;
                case Side.Right: return Side.Left;
                default: return Side.None;
            }
        }
    }
}
