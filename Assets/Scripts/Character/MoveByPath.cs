using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// パスに沿って移動させるクラス
/// </summary>
public class MoveByPath : MonoBehaviour
{
    /// <summary>
    /// 移動速度
    /// </summary>
    private float moveSpeed;
    /// <summary>
    /// 停止中か
    /// </summary>
    private bool isStop;
    /// <summary>
    /// 移動先のパスがない
    /// </summary>
    private bool isNoPath;
    /// <summary>
    /// 現在のパスにおける位置(0.0f - 1.0f)
    /// </summary>
    private float positionInPath;
    /// <summary>
    /// タイルの接続情報
    /// </summary>
    private TileConnection.PathDirectionInfo pathDirectionInfo;

    public CinemachinePathBase.PositionUnits positionUnits = CinemachinePathBase.PositionUnits.Distance;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        // 移動パスの終端に達したら次のタイルの準備を行う
        if (IsEndPosition())
        {
            SetupNextTile();
        }

        if (IsStop())
        {
            // 停止中の更新処理
            UpdateStop();
        }
        else
        {
            // 移動中の更新処理
            UpdateMove(Time.deltaTime);
        }
    }

    /// <summary>
    /// 指定されたタイルの接続情報をもとに、内部情報を更新する。
    /// </summary>
    /// <param name="info">タイルの接続情報</param>
    private void SetPathDirectionInfo(TileConnection.PathDirectionInfo info)
    {
        // 現在のタイル接続情報を交信
        if (info == null)
        {
            return;
        }
        pathDirectionInfo = info;

        // キャラクターの移動方向に合わせて、始点と終点を設定する
        if (pathDirectionInfo.direction == Core.Direction.Reverse)
        {
            positionInPath = pathDirectionInfo.path.MaxPos;
        }
        else
        {
            positionInPath = pathDirectionInfo.path.MinPos;
        }
    }

    /// <summary>
    /// 移動パスの終端に達しているかどうかチェックする。
    /// </summary>
    /// <returns>移動パスの終端に達している場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool IsEndPosition()
    {
        // パラメータの整合性チェック
        if (pathDirectionInfo == null)
        {
            return false;
        }
        if (pathDirectionInfo.path == null)
        {
            return false;
        }

        // キャラクターの移動方向に合わせて、始点or終点とチェックを行う
        if (pathDirectionInfo.direction == Core.Direction.Normal)
        {
            if (positionInPath < pathDirectionInfo.path.MaxPos)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (pathDirectionInfo.direction == Core.Direction.Reverse)
        {
            if (positionInPath > pathDirectionInfo.path.MinPos)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 次のタイルの準備を行う
    /// </summary>
    private void SetupNextTile()
    {
        // パラメータの整合性チェック
        if (pathDirectionInfo == null)
        {
            return;
        }
        if (pathDirectionInfo.tileConnection == null)
        {
            return;
        }

        // パスの終点に接続されている次のタイルを取得する
        GameObject nextTileObject = pathDirectionInfo.tileConnection.GetNeighborGameObject(pathDirectionInfo.pathEnd);
        if (nextTileObject == null)
        {
            isNoPath = true;    // 隣のタイルがない
            return;
        }

        // 隣のタイルの接続情報を取得
        TileConnection nextTileConnection = nextTileObject.GetComponent<TileConnection>();
        if (nextTileConnection == null)
        {
            isNoPath = true;    // 隣のタイルがない
            return;
        }

        // パスの接続先の辺を取得
        // 現在のパスの終点に対向する辺を取得し、その辺に接している接続情報を取得する。
        Core.Side nextSide = Core.Utils.Opposite(pathDirectionInfo.pathEnd);
        TileConnection.PathDirectionInfo info = nextTileConnection.GetPathDirectionInfo(nextSide);
        SetPathDirectionInfo(info);
    }

    /// <summary>
    /// 移動中の更新処理を行う。
    /// </summary>
    /// <param name="deltaTime">前回呼び出しからの経過時間</param>
    private void UpdateMove(float deltaTime)
    {
        // パラメータの整合性チェック
        if (pathDirectionInfo == null)
        {
            return;
        }
        if (pathDirectionInfo.path == null)
        {
            return;
        }
        CinemachinePathBase path = pathDirectionInfo.path;

        // キャラクターの移動方向に合わせて、次の移動位置、向きを更新する
        if (pathDirectionInfo.direction == Core.Direction.Reverse)
        {
            float distanceAlongPath = positionInPath - moveSpeed * deltaTime;

            positionInPath = path.StandardizeUnit(distanceAlongPath, positionUnits);
            transform.position = path.EvaluatePositionAtUnit(positionInPath, positionUnits);
            transform.rotation = path.EvaluateOrientationAtUnit(positionInPath, positionUnits);

            // 逆方向に動いているので正面方向のベクトルを逆にする
            Quaternion reverseQuart = Quaternion.AngleAxis(180.0f, Vector3.up);
            transform.rotation *= reverseQuart;
        }
        else
        {
            float distanceAlongPath = positionInPath + moveSpeed * deltaTime;

            positionInPath = path.StandardizeUnit(distanceAlongPath, positionUnits);
            transform.position = path.EvaluatePositionAtUnit(positionInPath, positionUnits);
            transform.rotation = path.EvaluateOrientationAtUnit(positionInPath, positionUnits);

        }
    }

    /// <summary>
    /// 停止時の更新処理
    /// </summary>
    private void UpdateStop()
    {
        // 手前を向かせる
        Quaternion reverseQuart = Quaternion.AngleAxis(180.0f, Vector3.up);
        transform.rotation = reverseQuart;
    }

    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    /// <param name="startPos">最初にキャラクターを置くタイルの座標</param>
    public void Initialize(Vector2Int startPos)
    {
        isStop = true;
        isNoPath = false;
        moveSpeed = 0.0f;

        // タイルのゲームを部ジェクトを取得
        GameObject tileObject = TileManager.Instance.FindTile(startPos.x, startPos.y);
        if (tileObject == null)
        {
            return;
        }

        // タイル接続情報を取得
        TileConnection tileConnection = tileObject.GetComponent<TileConnection>();
        if (tileConnection == null)
        {
            return;
        }

        Core.Side[] sides =
        {
            Core.Side.Up,
            Core.Side.Down,
            Core.Side.Left,
            Core.Side.Right,
        };

        // タイルの各辺を確認し、最初に接続情報を持つ辺が見つかったら
        // そのパスの始点にキャラクターをセットする。
        foreach (var side in sides)
        {
            if (!tileConnection.HasConnection(side))
            {
                continue;
            }

            TileConnection.PathDirectionInfo info = tileConnection.GetPathDirectionInfo(side);
            SetPathDirectionInfo(info);
            break;
        }

        UpdateMove(0);
    }

    /// <summary>
    /// 移動速度を取得する。
    /// </summary>
    /// <returns>移動速度</returns>
    public float GetSpeed()
    {
        return moveSpeed;
    }

    /// <summary>
    /// 移動速度を設定する。
    /// </summary>
    /// <param name="speed">移動速度</param>
    public void SetSpeed(float speed)
    {
        // 移動速度が0の場合は停止フラグを立てる。
        moveSpeed = speed;
        if (speed > 0.0f)
        {
            SetStopFlag(false);
        }
        else
        {
            SetStopFlag(true);
        }
    }

    /// <summary>
    /// 停止フラグをセットする
    /// </summary>
    /// <param name="flag">セットするフラグ</param>
    public void SetStopFlag(bool flag)
    {
        isStop = flag;
    }

    /// <summary>
    /// 停止中かどうかチェックする。
    /// </summary>
    /// <returns>停止中の場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsStop()
    {
        return isStop;
    }

    /// <summary>
    /// 次の移動先タイルがあるかどうかチェックする。
    /// </summary>
    /// <returns>次の移動先タイルがある場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsNoPath()
    {
        return isNoPath;
    }

    /// <summary>
    /// 現在いるタイルの座標を取得する
    /// </summary>
    /// <param name="pos">タイルの座標が格納される。</param>
    /// <returns>現在いるタイルがある場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool GetCurrentPosition(ref Vector2Int pos)
    {
        if (pathDirectionInfo == null)
        {
            return false;
        }
        if (pathDirectionInfo.tileConnection == null)
        {
            return false;
        }
        if (pathDirectionInfo.tileConnection.gameObject == null)
        {
            return false;
        }
        GameObject tileObject = pathDirectionInfo.tileConnection.gameObject;
        TileController tileController = tileObject.GetComponent<TileController>();
        if (tileController == null)
        {
            return false;
        }

        pos = tileController.GetPosition();
        return true;
    }
}
