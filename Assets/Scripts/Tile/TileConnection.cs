using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// タイルのパス情報、接続情報を管理するクラス
/// </summary>
public class TileConnection : MonoBehaviour
{
    /// <summary>
    /// パス情報
    /// </summary>
    [System.Serializable]
    public struct PathInfo
    {
        /// <summary>
        /// パスデータ
        /// </summary>
        public CinemachinePathBase path;
        /// <summary>
        /// 接続元の辺
        /// </summary>
        public Core.Side pathFrom;
        /// <summary>
        /// 接続先の辺
        /// </summary>
        public Core.Side pathTo;
    }

    /// <summary>
    /// パス情報(方向付き)
    /// </summary>
    public class PathDirectionInfo
    {
        /// <summary>
        /// 自身のクラス
        /// </summary>
        public TileConnection tileConnection;
        /// <summary>
        /// パスデータ
        /// </summary>
        public CinemachinePathBase path;
        /// <summary>
        /// パスの始点
        /// </summary>
        public Core.Side pathBegin;
        /// <summary>
        /// パスの終点
        /// </summary>
        public Core.Side pathEnd;
        /// <summary>
        /// パスの方向
        /// </summary>
        public Core.Direction direction;
    }

    /// <summary>
    /// タイルの種類
    /// </summary>
    [SerializeField] private Core.Tile tileType;
    /// <summary>
    /// パス情報リスト
    /// </summary>
    [SerializeField] private List<PathInfo> pathInfos;

    /// <summary>
    /// 接続先マップ
    /// </summary>
    private Dictionary<Core.Side, GameObject> connections;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        connections = new Dictionary<Core.Side, GameObject>();
    }

    /// <summary>
    /// 指定した Core.Side に接続されているパスを取得する。
    /// </summary>
    /// <param name="side">どの辺に接続されているか</param>
    /// <returns>接続されているパスを返す。ない場合はnullを返す。</returns>
    public CinemachinePathBase GetPath(Core.Side side)
    {
        foreach (var info in pathInfos)
        {
            if (info.pathFrom == side)
            {
                return info.path;
            }
            else if (info.pathTo == side)
            {
                return info.path;
            }
        }
        return null;
    }

    /// <summary>
    /// 指定した Core.Side に接続されているパスの向きを取得する。
    /// </summary>
    /// <param name="side">どの辺に接続されているか</param>
    /// <returns>入り口に接続されている場合はNormal、
    /// 出口の場合はReverse、接続されていない場合はNoneを返す。</returns>
    public Core.Direction GetDirection(Core.Side side)
    {
        foreach (var info in pathInfos)
        {
            if (info.pathFrom == side)
            {
                return Core.Direction.Normal;
            }
            else if (info.pathTo == side)
            {
                return Core.Direction.Reverse;
            }
        }
        return Core.Direction.None;
    }

    /// <summary>
    /// 指定した Core.Side に接続先があるかをチェックする。
    /// </summary>
    /// <param name="side">どの辺に接続されているか</param>
    /// <returns>接続先がある場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool HasConnection(Core.Side side)
    {
        foreach (var info in pathInfos)
        {
            if (info.pathFrom == side)
            {
                return true;
            }
            else if (info.pathTo == side)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 指定した辺に接続されているパスの逆側の辺を取得する。
    /// </summary>
    /// <param name="side">どの辺に接続されているか</param>
    /// <returns>逆側の辺を返す。ない場合はCore.Side.Noneを返す。</returns>
    public Core.Side GetOppositeSide(Core.Side side)
    {
        foreach (var info in pathInfos)
        {
            if (info.pathFrom == side)
            {
                return info.pathTo;
            }
            else if (info.pathTo == side)
            {
                return info.pathFrom;
            }
        }
        return Core.Side.None;
    }

    public PathDirectionInfo GetPathDirectionInfo(Core.Side side)
    {
        PathDirectionInfo info = new PathDirectionInfo();
        info.tileConnection = this;
        info.path = GetPath(side);
        info.pathBegin = side;
        info.pathEnd = GetOppositeSide(side);
        info.direction = GetDirection(side);

        return info;
    }

    /// <summary>
    /// 指定した Core.Side の接続先のGameObjectを取得する
    /// </summary>
    /// <param name="side">どの辺に接続されているか</param>
    /// <returns>接続先がある場合はそのGameObjectを返す。ない場合はnullを返す。</returns>
    public GameObject GetNeighborGameObject(Core.Side side)
    {
        if (!HasConnection(side))
        {
            return null;
        }

        if (!connections.ContainsKey(side))
        {
            return null;
        }

        return connections[side];
    }

    /// <summary>
    /// 接続先マップをクリアする。
    /// </summary>
    public void ClearConnection()
    {
        connections.Clear();
    }

    /// <summary>
    /// 接続先マップに追加する。
    /// すでに接続先情報がある場合は、上書きされる。
    /// </summary>
    /// <param name="side">どの辺に接続されているか</param>
    /// <param name="connectObject">接続先のゲームオブジェクト</param>
    public void AddConnection(Core.Side side, GameObject connectObject)
    {
        if (connections.ContainsKey(side))
        {
            connections.Remove(side);
        }
        connections.Add(side, connectObject);
    }

    [ContextMenu("DumpConnection")]
    public void DumpConnection()
    {
        foreach (var con in connections)
        {
            Core.Side side = con.Key;
            GameObject obj = con.Value;

            TileController tileController = obj.GetComponent<TileController>();
            Vector2Int pos = tileController.GetPosition();
            Debug.LogFormat("side:{0} pos:[{1},{2}]", side, pos.x, pos.y);
        }
    }
}
