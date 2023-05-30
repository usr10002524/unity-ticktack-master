using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// エイリアス
using SpPat = Core.SpawnPattern;
using SpPatArea = Core.SpawnPatternArea;

/// <summary>
/// タイル管理クラス
/// </summary>
public class TileManager : MonoBehaviour
{
    /// <summary>
    /// 横方向のタイル数
    /// </summary>
    [SerializeField] private int cols;
    /// <summary>
    /// 縦方向のタイル数
    /// </summary>
    [SerializeField] private int rows;
    /// <summary>
    /// タイルオブジェクトのベース
    /// </summary>
    [SerializeField] private GameObject tileBase;
    /// <summary>
    /// タイルオブジェクトのプレファブリスト
    /// </summary>
    [SerializeField] private List<GameObject> tilePrefabs;
    /// <summary>
    /// 背景オブジェクトのプレファブ
    /// </summary>
    [SerializeField] private GameObject bgPrefab;

    /// <summary>
    /// タイルオブジェクトのリスト
    /// </summary>
    private List<GameObject> tiles;
    /// <summary>
    /// 背景オブジェクト
    /// </summary>
    private GameObject bgObject;
    /// <summary>
    /// 空白の箇所
    /// </summary>
    private Vector2Int blankPosition;

    /// <summary>
    /// 移動情報
    /// </summary>
    private class MoveInfo
    {
        public GameObject obj;
        public Vector2Int position;
    }

    /// <summary>
    /// 移動情報リスト
    /// </summary>
    private List<MoveInfo> moveInfos;
    /// <summary>
    /// 移動用コルーチン
    /// </summary>
    private Coroutine moveCoroutine;

    /// <summary>
    /// スポーン位置定義
    /// </summary>
    private class SpawnPositionDef
    {
        /// <summary>
        /// スポーンパターン
        /// </summary>
        public Core.SpawnPattern pattern;
        /// <summary>
        /// パターンに含まれるエリア
        /// </summary>
        public Core.SpawnPatternArea[] containArea;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pattern">スポーンパターン</param>
        /// <param name="containArea">パターンに含まれるエリア</param>
        public SpawnPositionDef(Core.SpawnPattern pattern, Core.SpawnPatternArea[] containArea)
        {
            this.pattern = pattern;
            this.containArea = containArea;
        }
    }

    /// <summary>
    /// スポーン位置定義のリスト
    /// </summary>
    private static readonly SpawnPositionDef[] spawnPositionDefs = {
            new SpawnPositionDef(SpPat.C, new SpPatArea[]{SpPatArea.C}),
            new SpawnPositionDef(SpPat.UL, new SpPatArea[]{SpPatArea.UL}),
            new SpawnPositionDef(SpPat.UR, new SpPatArea[]{SpPatArea.UR}),
            new SpawnPositionDef(SpPat.DL, new SpPatArea[]{SpPatArea.DL}),
            new SpawnPositionDef(SpPat.DR, new SpPatArea[]{SpPatArea.DR}),

            new SpawnPositionDef(SpPat.V2, new SpPatArea[]{SpPatArea.U, SpPatArea.D}),
            new SpawnPositionDef(SpPat.H2, new SpPatArea[]{SpPatArea.L, SpPatArea.R}),
            new SpawnPositionDef(SpPat.UL2, new SpPatArea[]{SpPatArea.U, SpPatArea.L}),
            new SpawnPositionDef(SpPat.UR2, new SpPatArea[]{SpPatArea.U, SpPatArea.R}),
            new SpawnPositionDef(SpPat.DL2, new SpPatArea[]{SpPatArea.D, SpPatArea.L}),
            new SpawnPositionDef(SpPat.DR2, new SpPatArea[]{SpPatArea.D, SpPatArea.R}),

            new SpawnPositionDef(SpPat.V3, new SpPatArea[]{SpPatArea.U, SpPatArea.C, SpPatArea.D}),
            new SpawnPositionDef(SpPat.H3, new SpPatArea[]{SpPatArea.L, SpPatArea.C, SpPatArea.R}),
            new SpawnPositionDef(SpPat.UL3, new SpPatArea[]{SpPatArea.U, SpPatArea.UL, SpPatArea.L}),
            new SpawnPositionDef(SpPat.UR3, new SpPatArea[]{SpPatArea.U, SpPatArea.UR, SpPatArea.R}),
            new SpawnPositionDef(SpPat.DL3, new SpPatArea[]{SpPatArea.D, SpPatArea.DL, SpPatArea.L}),
            new SpawnPositionDef(SpPat.DR3, new SpPatArea[]{SpPatArea.D, SpPatArea.DR, SpPatArea.R}),

        };

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    public static TileManager Instance { get; private set; }


    /// <summary>
    /// 横方向のタイルの数を取得する。
    /// </summary>
    /// <returns>横方向のタイルの数</returns>
    public int GetCols()
    {
        return cols;
    }

    /// <summary>
    /// 縦方向のタイルの数を取得する。
    /// </summary>
    /// <returns>縦方向のタイルの数</returns>
    public int GetRows()
    {
        return rows;
    }

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        tiles = new List<GameObject>();
        blankPosition = new Vector2Int(-1, -1);

        moveInfos = new List<MoveInfo>();
    }

    /// <summary>
    /// タイルオブジェクトの位置を設定する。
    /// </summary>
    /// <param name="gameObject">タイルオブジェクト</param>
    /// <param name="position">設定する位置</param>
    private void SetPosition(GameObject gameObject, Vector2Int position)
    {
        TileController tileController = gameObject.GetComponent<TileController>();
        if (tileController == null)
        {
            return;
        }

        tileController.SetPosition(position);
    }

    /// <summary>
    /// タイルオブジェクトの位置を取得する
    /// </summary>
    /// <param name="gameObject">タイルオブジェクト</param>
    /// <param name="position">位置が格納される</param>
    /// <returns>位置が正常に取得できた場合はtrueを、そうでない場合はfalseを返す。</returns>
    private bool GetPosition(GameObject gameObject, ref Vector2Int position)
    {
        TileController tileController = gameObject.GetComponent<TileController>();
        if (tileController == null)
        {
            return false;
        }

        position = tileController.GetPosition();
        return true;
    }

    /// <summary>
    /// タイルをシャッフルする
    /// </summary>
    private void Shuffle()
    {
        Vector2Int[] posArray = {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(0,2),
            new Vector2Int(0,3),
            new Vector2Int(1,0),
            new Vector2Int(1,3),
            new Vector2Int(2,0),
            new Vector2Int(2,3),
            new Vector2Int(3,0),
            new Vector2Int(3,1),
            new Vector2Int(3,2),
            new Vector2Int(3,3),
        };

        int shuffleCount = 128;

        for (int i = 0; i < shuffleCount; i++)
        {
            int fromIndex = Random.Range(0, 1048576) % posArray.Length;
            int toIndex = Random.Range(0, 1048576) % posArray.Length;
            if (fromIndex == toIndex)
            {
                continue;
            }

            Vector2Int fromPos = posArray[fromIndex];
            Vector2Int toPos = posArray[toIndex];

            GameObject fromObj = FindTile(fromPos.x, fromPos.y);
            GameObject toObj = FindTile(toPos.x, toPos.y);

            // 位置を入れ替える
            SetPosition(fromObj, toPos);
            SetPosition(toObj, fromPos);
        }
    }

    /// <summary>
    /// タイルの初期化を行う
    /// </summary>
    private void InitTile()
    {
        ClearTile();

        int prefabIndex = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject prefab = tilePrefabs[prefabIndex];
                prefabIndex++;

                GameObject tileObject = Instantiate(prefab, tileBase.transform);
                tileObject.transform.position = GetTilePosition(c, r);

                TileController tileController = tileObject.GetComponent<TileController>();
                tileController.SetPosition(new Vector2Int(c, r));

                tiles.Add(tileObject);
            }
        }
    }

    /// <summary>
    /// タイルオブジェクトを破棄する
    /// </summary>
    private void ClearTile()
    {
        foreach (var tile in tiles)
        {
            Destroy(tile);
        }
        tiles.Clear();
    }

    /// <summary>
    /// ブランクタイルを設定する
    /// </summary>
    private void InitBlankTile()
    {
        if (blankPosition.x >= 0 || blankPosition.y >= 0)
        {
            return; // すでにブランクタイルを設定済み
        }

        Vector2Int[] posArray = {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(0,2),
            new Vector2Int(0,3),
            new Vector2Int(1,0),
            new Vector2Int(1,3),
            new Vector2Int(2,0),
            new Vector2Int(2,3),
            new Vector2Int(3,0),
            new Vector2Int(3,1),
            new Vector2Int(3,2),
            new Vector2Int(3,3),
        };

        int index = Random.Range(0, posArray.Length);
        Vector2Int pos = posArray[index];

        SetBlackTile(pos.x, pos.y);
    }

    /// <summary>
    /// 指定位置をブランクタイルにする
    /// </summary>
    /// <param name="col">横位置</param>
    /// <param name="row">縦位置</param>
    private void SetBlackTile(int col, int row)
    {
        if (blankPosition.x >= 0 || blankPosition.y >= 0)
        {
            return; // すでにブランクタイルを設定済み
        }

        GameObject target = FindTile(col, row);
        if (target != null)
        {
            tiles.Remove(target);
            Destroy(target);
            blankPosition = new Vector2Int(col, row);
        }
    }

    /// <summary>
    /// 背景タイルを作成する。
    /// </summary>
    private void InitBgTile()
    {
        if (bgPrefab == null)
        {
            return;
        }
        if (bgObject != null)
        {
            Destroy(bgObject);
            bgObject = null;
        }
        bgObject = Instantiate(bgPrefab, tileBase.transform);
    }


    /// <summary>
    /// タイルマップ全体のサイズを取得する。
    /// </summary>
    /// <returns>タイルマップ全体のサイズ</returns>
    public Vector2 GetWorldSize()
    {
        float worldWidth = Core.Consts.TileWidth * cols;
        float worldHeight = Core.Consts.TileHeight * rows;
        return new Vector2(worldWidth, worldHeight);
    }

    /// <summary>
    /// タイルマップ(0,0)の座標を取得する。
    /// </summary>
    /// <returns>タイルマップ(0,0)の座標</returns>
    public Vector2 GetOrigPosition()
    {
        Vector2 worldSize = GetWorldSize();
        Vector2 step = GetTileStep();

        float originX = -worldSize.x * 0.5f + step.x * 0.5f;
        float originZ = worldSize.y * 0.5f + step.y * 0.5f;
        return new Vector2(originX, originZ);
    }

    /// <summary>
    /// タイルマップ縦横それぞれ1マス移動するためのベクトルを取得する。
    /// </summary>
    /// <returns>1マス移動するためのベクトル</returns>
    public Vector2 GetTileStep()
    {
        float stepCol = Core.Consts.TileWidth;
        float stepRow = -Core.Consts.TileHeight;
        return new Vector2(stepCol, stepRow);
    }

    /// <summary>
    /// タイルマップの指定位置の座標を取得する。
    /// </summary>
    /// <param name="col">横位置</param>
    /// <param name="row">縦位置</param>
    /// <returns>タイルの座標</returns>
    public Vector3 GetTilePosition(int col, int row)
    {
        Vector2 orig = GetOrigPosition();
        Vector2 step = GetTileStep();

        return new Vector3(orig.x + step.x * col, 0, orig.y + step.y * row);
    }

    /// <summary>
    /// 指定パターンに含まれるエリアのリストを取得する
    /// </summary>
    /// <param name="pattern">パターン</param>
    /// <returns>エリアのリスト</returns>
    public Core.SpawnPatternArea[] GetSpawnPatternAreas(Core.SpawnPattern pattern)
    {
        foreach (var defs in spawnPositionDefs)
        {
            if (defs.pattern == pattern)
            {
                return defs.containArea;
            }
        }

        return new Core.SpawnPatternArea[] { };
    }

    /// <summary>
    /// 指定したタイル座標、エリアの3D座標を取得する
    /// </summary>
    /// <param name="col">横位置</param>
    /// <param name="row">縦位置</param>
    /// <param name="area">エリア</param>
    /// <returns>3D座標</returns>
    public Vector3 GetSpawnPosition(int col, int row, Core.SpawnPatternArea area)
    {
        Vector3 pos = GetTilePosition(col, row);
        Vector3 offset = GetSpawnOffset(area);
        pos += offset;

        return pos;
    }

    /// <summary>
    /// スポーン場所のオフセットを取得する。
    /// </summary>
    /// <param name="area">スポーン場所</param>
    /// <returns>スポーン場所へのオフセット</returns>
    private Vector3 GetSpawnOffset(Core.SpawnPatternArea area)
    {
        switch (area)
        {
            case Core.SpawnPatternArea.C: return new Vector3(0.0f, 0.0f, 0.0f);

            case Core.SpawnPatternArea.U: return new Vector3(0.0f, 0.0f, 1.6f);
            case Core.SpawnPatternArea.D: return new Vector3(0.0f, 0.0f, -1.6f);
            case Core.SpawnPatternArea.L: return new Vector3(-1.6f, 0.0f, 0.0f);
            case Core.SpawnPatternArea.R: return new Vector3(1.6f, 0.0f, 0.0f);

            case Core.SpawnPatternArea.UL: return new Vector3(-0.8f, 0.0f, 0.8f);
            case Core.SpawnPatternArea.UR: return new Vector3(0.8f, 0.0f, 0.8f);
            case Core.SpawnPatternArea.DL: return new Vector3(-0.8f, 0.0f, -0.8f);
            case Core.SpawnPatternArea.DR: return new Vector3(0.8f, 0.0f, -0.8f);

            default: return new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    /// <summary>
    /// タイルマップの初期化を行う。
    /// </summary>
    public void Initialize()
    {
        // Debug.LogFormat("TileManager.Slide() called.");

        if (null == tileBase)
        {
            throw new System.ArgumentException("tileBase is null.");
        }

        int tileCount = cols * rows;
        if (0 == tileCount)
        {
            throw new System.ArgumentException(string.Format("cols or rows is zero. cols:{0} rows:{1}", cols, rows));
        }
        if (tileCount > tilePrefabs.Count)
        {
            throw new System.ArgumentException(string.Format("TilePrefabs count not enough. tileCount:{0} TilePrefabs:{1}", tileCount, tilePrefabs.Count));
        }

        moveInfos.Clear();
        blankPosition = new Vector2Int(-1, -1);

        // タイルを配置する
        InitTile();
        // シャッフルする
        Shuffle();
        // ブランクタイルを設定する
        InitBlankTile();
        // 背景タイルを作成する
        InitBgTile();
        // 接続情報を更新する
        UpdateConnection();
        // 表示を更新する
        UpdateTilemap();
    }

    /// <summary>
    /// タイルマップの初期化を行う。（TitleScene用）
    /// </summary>
    public void InitializeTitleScene()
    {
        // Debug.LogFormat("TileManager.Slide() called.");

        if (null == tileBase)
        {
            throw new System.ArgumentException("tileBase is null.");
        }

        int tileCount = cols * rows;
        if (0 == tileCount)
        {
            throw new System.ArgumentException(string.Format("cols or rows is zero. cols:{0} rows:{1}", cols, rows));
        }
        if (tileCount > tilePrefabs.Count)
        {
            throw new System.ArgumentException(string.Format("TilePrefabs count not enough. tileCount:{0} TilePrefabs:{1}", tileCount, tilePrefabs.Count));
        }

        moveInfos.Clear();
        blankPosition = new Vector2Int(-1, -1);

        // タイルを配置する
        InitTile();
        // シャッフルする
        // Shuffle();
        // ブランクタイルを設定する
        SetBlackTile(1, 2);
        // 背景タイルを作成する
        InitBgTile();
        // 接続情報を更新する
        UpdateConnection();
        // 表示を更新する
        UpdateTilemap();
    }

    /// <summary>
    /// タイルマップを更新する
    /// </summary>
    public void UpdateTilemap()
    {
        foreach (var item in tiles)
        {
            TileController tileController = item.GetComponent<TileController>();
            tileController.UpdatePosition();
        }
    }

    /// <summary>
    /// 接続情報を更新する
    /// </summary>
    public void UpdateConnection()
    {
        Core.Side[] sides = {
            Core.Side.Up,
            Core.Side.Down,
            Core.Side.Left,
            Core.Side.Right,
        };

        foreach (var item in tiles)
        {
            TileConnection tileConnection = item.GetComponent<TileConnection>();
            if (tileConnection == null)
            {
                continue;
            }
            tileConnection.ClearConnection();

            TileController tileController = item.GetComponent<TileController>();
            if (tileController == null)
            {
                continue;
            }

            Vector2Int position = tileController.GetPosition();

            foreach (var side in sides)
            {
                if (!tileConnection.HasConnection(side))
                {
                    continue;   // 接続情報がないのでスキップ
                }

                Vector2Int neighbor = Core.Utils.Offset(position, side);
                GameObject neighborObject = FindTile(neighbor.x, neighbor.y);
                if (neighborObject == null)
                {
                    continue;   // 指定座標にゲームオブジェクトがないのでスキップ
                }

                TileConnection neighborTileConnection = neighborObject.GetComponent<TileConnection>();
                if (neighborTileConnection == null)
                {
                    continue;   // 隣接タイルマップにTileConnectionがないのでスキップ
                }

                Core.Side oppositeSide = Core.Utils.Opposite(side);
                if (!neighborTileConnection.HasConnection(oppositeSide))
                {
                    continue;   // 隣接オブジェクトにコネクションがないのでスキップ
                }

                // 接続情報を設定
                tileConnection.AddConnection(side, neighborObject);
            }

        }
    }

    /// <summary>
    /// 指定位置のタイルオブジェクトを取得する
    /// </summary>
    /// <param name="col">横座標</param>
    /// <param name="row">縦座標</param>
    /// <returns>指定位置にタイルがある場合はそのオブジェクトを返す。ない場合はnullを返す。</returns>
    public GameObject FindTile(int col, int row)
    {
        return tiles.Find(
            delegate (GameObject obj)
            {
                TileController tileController = obj.GetComponent<TileController>();
                if (obj != null)
                {
                    Vector2Int pos = tileController.GetPosition();
                    return (pos.x == col && pos.y == row);
                }
                return false;
            }
        );
    }

    /// <summary>
    /// 指定座標がスライド可能かどうかをチェックする。
    /// 縦横の座標がブランクタイルと一致し、ブランクタイルの場所ではないときにスライド可能。
    /// </summary>
    /// <param name="col">横座標</param>
    /// <param name="row">縦座標</param>
    /// <returns>スライド可能な座標の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsSlideEnable(int col, int row)
    {
        // Debug.LogFormat("TileManager.IsSlideEnable() called. x:{0} y:{1}", col, row);

        if (blankPosition.x == col && blankPosition.y == row)
        {
            return false;
        }

        return (blankPosition.x == col || blankPosition.y == row);
    }

    /// <summary>
    /// 指定座標からスライドさせる。
    /// </summary>
    /// <param name="col">横位置</param>
    /// <param name="row">縦位置</param>
    public void Slide(int col, int row)
    {
        if (!IsSlideEnable(col, row))
        {
            return;
        }

        if (col < blankPosition.x)
        {
            for (int c = blankPosition.x - 1; c >= col; c--)
            {
                GameObject tileObject = FindTile(c, row);
                if (tileObject != null)
                {
                    SetPosition(tileObject, new Vector2Int(c + 1, row));
                }
            }
        }
        else if (col > blankPosition.x)
        {
            for (int c = blankPosition.x + 1; c <= col; c++)
            {
                GameObject tileObject = FindTile(c, row);
                if (tileObject != null)
                {
                    SetPosition(tileObject, new Vector2Int(c - 1, row));
                }
            }
        }
        else if (row < blankPosition.y)
        {
            for (int r = blankPosition.y - 1; r >= row; r--)
            {
                GameObject tileObject = FindTile(col, r);
                if (tileObject != null)
                {
                    SetPosition(tileObject, new Vector2Int(col, r + 1));
                }
            }
        }
        else if (row > blankPosition.y)
        {
            for (int r = blankPosition.y + 1; r <= row; r++)
            {
                GameObject tileObject = FindTile(col, r);
                if (tileObject != null)
                {
                    SetPosition(tileObject, new Vector2Int(col, r - 1));
                }
            }
        }

        blankPosition = new Vector2Int(col, row);
    }

    /// <summary>
    /// アニメーション付きでスライドさせる。
    /// </summary>
    /// <param name="col">横位置</param>
    /// <param name="row">縦位置</param>
    public void MoveSlide(int col, int row)
    {
        // Debug.LogFormat("TileManager.MoveSlide() called.");
        if (!IsSlideEnable(col, row))
        {
            // Debug.LogFormat("TileManager.MoveSlide() IsSlideEnable is false.");
            return;
        }

        moveInfos.Clear();

        if (col < blankPosition.x)
        {
            for (int c = blankPosition.x - 1; c >= col; c--)
            {
                GameObject tileObject = FindTile(c, row);
                MoveOneTile(tileObject, Core.Side.Right);
            }
        }
        else if (col > blankPosition.x)
        {
            for (int c = blankPosition.x + 1; c <= col; c++)
            {
                GameObject tileObject = FindTile(c, row);
                MoveOneTile(tileObject, Core.Side.Left);
            }
        }
        else if (row < blankPosition.y)
        {
            for (int r = blankPosition.y - 1; r >= row; r--)
            {
                GameObject tileObject = FindTile(col, r);
                MoveOneTile(tileObject, Core.Side.Down);
            }
        }
        else if (row > blankPosition.y)
        {
            for (int r = blankPosition.y + 1; r <= row; r++)
            {
                GameObject tileObject = FindTile(col, r);
                MoveOneTile(tileObject, Core.Side.Up);
            }
        }

        if (moveInfos.Count == 0)
        {
            return;
        }

        blankPosition = new Vector2Int(col, row);
        TileManager.Instance.UpdateConnection();
        StartMoveCoroutine();
    }

    /// <summary>
    /// 位置タイルの移動処理を行う
    /// </summary>
    /// <param name="obj">タイルオブジェクト</param>
    /// <param name="side">移動方向</param>
    private void MoveOneTile(GameObject obj, Core.Side side)
    {
        // Debug.LogFormat("TileManager.MoveOneTile() called.");

        if (obj == null)
        {
            return;
        }

        TileController tileController = obj.GetComponent<TileController>();
        if (tileController == null)
        {
            return;
        }

        EasePosition easePosition = obj.GetComponent<EasePosition>();
        if (easePosition == null)
        {
            return;
        }

        Vector2Int pos = tileController.GetPosition();
        Vector2Int neighbor = Core.Utils.Offset(pos, side);

        Vector3 startPos = TileManager.Instance.GetTilePosition(pos.x, pos.y);
        Vector3 endPos = TileManager.Instance.GetTilePosition(neighbor.x, neighbor.y);
        // Debug.LogFormat("TileManager.MoveOneTile() [{4}] ({0},{1}) -> ({2},{3})", pos.x, pos.y, neighbor.x, neighbor.y, moveInfos.Count);

        easePosition.SetParameter(startPos, endPos);
        easePosition.StartEasing();

        MoveInfo moveInfo = new MoveInfo();
        moveInfo.obj = obj;
        moveInfo.position = neighbor;
        moveInfos.Add(moveInfo);
        tileController.SetPosition(neighbor);
    }

    /// <summary>
    /// 移動中かどうか確認する。
    /// </summary>
    /// <returns>移動中の場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsInEasing()
    {
        foreach (var info in moveInfos)
        {
            Easing easing = info.obj.GetComponent<Easing>();
            if (easing == null)
            {
                continue;
            }

            if (easing.IsInEasing())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 移動アニメのコルーチンを起動する。
    /// </summary>
    private void StartMoveCoroutine()
    {
        if (moveInfos.Count == 0)
        {
            return;
        }

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        moveCoroutine = StartCoroutine("MoveCoroutine");
    }

    /// <summary>
    /// 移動アニメのコルーチンを停止する
    /// </summary>
    private void EndMoveCoroutine()
    {
        if (moveCoroutine != null)
        {
            TileManager.Instance.UpdateTilemap();

            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    /// <summary>
    /// 移動コルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator MoveCoroutine()
    {
        bool isEnd = false;

        // SeManager.Instance.PlaySe(SeType.seSlide);

        while (isEnd)
        {
            yield return null;

            if (!IsInEasing())
            {
                isEnd = true;
            }
        }

        EndMoveCoroutine();
    }
}
