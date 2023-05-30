using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// スポーン管理クラス
/// </summary>
public class SpawnManager : MonoBehaviour
{
    /// <summary>
    /// スポーン情報
    /// </summary>
    [System.Serializable]
    public class SpawnObjects
    {
        /// <summary>
        /// スポーンの種別
        /// </summary>
        public Core.SpawnType type;

        /// <summary>
        /// スポーンするオブジェクト
        /// </summary>
        public GameObject spawnPrefab;
        /// <summary>
        /// 出現ウェイト
        /// </summary>
        public int weight;
        /// <summary>
        /// 獲得スコア
        /// </summary>
        public int score;
        /// <summary>
        /// 獲得Rate
        /// </summary>
        public float rate;
        /// <summary>
        /// 1回あたりの出現最大数(0は無制限)
        /// </summary>
        public int maxPerWave;
    }

    /// <summary>
    /// スポーンパターン抽選のパラメータ
    /// </summary>
    [System.Serializable]
    public class SpawnPatternParam
    {
        /// <summary>
        /// スポーンパターン
        /// </summary>
        public Core.SpawnPattern pattern;
        /// <summary>
        /// 出現ウェイト
        /// </summary>
        public int weight;
    }

    /// <summary>
    /// スポーン結果
    /// </summary>
    private class SpawnResult
    {
        /// <summary>
        /// 出現タイルの座標
        /// </summary>
        public int row;
        /// <summary>
        /// 出現タイルの座標
        /// </summary>
        public int col;
        /// <summary>
        /// スポーンの種別
        /// </summary>
        public Core.SpawnType type;
        /// <summary>
        /// スポーンエリアの種別
        /// </summary>
        public Core.SpawnPatternArea area;
    }

    /// <summary>
    /// スポーンオブジェクトのベース
    /// </summary>
    [SerializeField] private GameObject spawnBase;
    /// <summary>
    /// スポーン間隔
    /// </summary>
    [SerializeField] private float spawnInterval;
    /// <summary>
    /// スポーンオブジェクトのリスト
    /// </summary>
    [SerializeField] private List<SpawnObjects> spawnObjects;
    /// <summary>
    /// スポーン抽選のパラメータ
    /// </summary>
    [SerializeField] private List<SpawnPatternParam> spawnPatternParams;

    /// <summary>
    /// ポーズフラグ
    /// </summary>
    private bool isPause;
    /// <summary>
    /// 出現ウェイトリスト
    /// </summary>
    private List<int> probs;
    /// <summary>
    /// パターンウェイトリスト
    /// </summary>
    private List<int> patternProbs;
    /// <summary>
    /// スポーン結果リスト
    /// </summary>
    private List<SpawnResult> spawnResults;
    /// <summary>
    /// コルーチン
    /// </summary>
    private Coroutine spawnCoroutine;
    /// <summary>
    /// スポーン数
    /// </summary>
    private int spawnCount;

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    public static SpawnManager Instance { get; private set; }


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

        probs = new List<int>();
        patternProbs = new List<int>();
        spawnResults = new List<SpawnResult>();
    }

    /// <summary>
    /// スポーンコルーチンを起動する
    /// </summary>
    public void StartSpawnCoroutine()
    {
        StopSpawnCoroutine();

        spawnCoroutine = StartCoroutine("SpawnCoroutine");
    }

    /// <summary>
    /// スポーンコルチーンを終了する
    /// </summary>
    public void StopSpawnCoroutine()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = null;
    }


    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        isPause = true;
        DestroySpawnObject();
        probs.Clear();
        patternProbs.Clear();

        foreach (var obj in spawnObjects)
        {
            probs.Add(obj.weight);
        }
        foreach (var obj in spawnPatternParams)
        {
            patternProbs.Add(obj.weight);
        }

        spawnCount = 0;

        // 初回スポーン
        Spawn();
        // 定期スポーン
        StartSpawnCoroutine();
    }

    /// <summary>
    /// ポーズ中かどうかチェックする。
    /// </summary>
    /// <returns>ポーズ中の場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsPause()
    {
        return isPause;
    }

    /// <summary>
    /// ポーズ状態にする。
    /// </summary>
    public void Pause()
    {
        isPause = true;
    }

    /// <summary>
    /// ポーズ状態を解除する。
    /// </summary>
    public void Resume()
    {
        isPause = false;
    }

    /// <summary>
    /// アイテムタイプに対応するスコアを取得する
    /// </summary>
    /// <param name="type">アイテムタイプ</param>
    /// <returns>スコア</returns>
    public int GetScore(Core.SpawnType type)
    {
        foreach (var obj in spawnObjects)
        {
            if (type == obj.type)
            {
                return obj.score;
            }
        }
        return 0;
    }

    /// <summary>
    /// アイテムタイプに対応するスコアレートを取得する。
    /// </summary>
    /// <param name="type">アイテムタイプ</param>
    /// <returns>スコアレート</returns>
    public float GetScoreRate(Core.SpawnType type)
    {
        foreach (var obj in spawnObjects)
        {
            if (type == obj.type)
            {
                return obj.rate;
            }
        }
        return 0.0f;
    }


    /// <summary>
    /// スポーン処理を行う
    /// </summary>
    private void Spawn()
    {
        int cols = TileManager.Instance.GetCols();
        int rows = TileManager.Instance.GetRows();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // 配置パターンを抽選
                int index = Lottery.LotteryWeight(patternProbs);
                Core.SpawnPattern pattern = spawnPatternParams[index].pattern;
                // タイルに配置を行う
                SpawnObjectByPattern(c, r, pattern);
            }
        }
    }

    /// <summary>
    /// 指定した場所に指定したパターンでスポーンさせる。
    /// </summary>
    /// <param name="col">タイルの座標</param>
    /// <param name="row">タイルの座標</param>
    /// <param name="pattern">スポーンのパターン</param>
    private void SpawnObjectByPattern(int col, int row, Core.SpawnPattern pattern)
    {
        // 配置箇所を取得
        Core.SpawnPatternArea[] areas = TileManager.Instance.GetSpawnPatternAreas(pattern);
        areas = GetAdjustArea(col, row, areas);

        // 各配置箇所にオブジェクトを配置する
        foreach (var area in areas)
        {
            // 出現オブジェクトの抽選
            SpawnObjects spawnObject = null;
            {
                int index = Lottery.LotteryWeight(probs);
                spawnObject = spawnObjects[index];
            }

            // 出現上限チェック
            if (spawnObject.maxPerWave > 0)
            {
                int spawnCount = GetSpawnCountInWave(spawnObject.type);
                if (spawnCount >= spawnObject.maxPerWave)
                {
                    continue; // Wave当たりの最大出現数を越えたのでスキップ
                }
            }

            // 配置済チェック
            // if (IsAlreadyPlaced(col, row, area))
            // {
            //     continue; // 指定場所にはすでに配置済
            // }

            Vector3 position = TileManager.Instance.GetSpawnPosition(col, row, area);
            GameObject childObject = Instantiate(spawnObject.spawnPrefab, spawnBase.transform);
            childObject.transform.position = position;

            ItemController itemController = childObject.GetComponent<ItemController>();
            if (itemController != null)
            {
                itemController.SetSpawnType(spawnObject.type);
            }

            SpawnResult result = new SpawnResult();
            result.col = col;
            result.row = row;
            result.area = area;
            result.type = spawnObject.type;

            spawnResults.Add(result);
        }
    }

    private Core.SpawnPatternArea[] GetAdjustArea(int col, int row, Core.SpawnPatternArea[] areas)
    {
        if (col == 1 && row == 1)
        {
            areas = areas.Where(
                delegate (Core.SpawnPatternArea area)
                {
                    switch (area)
                    {
                        case Core.SpawnPatternArea.R:
                        case Core.SpawnPatternArea.DR:
                        case Core.SpawnPatternArea.D:
                            return false;
                        default:
                            return true;
                    }
                }
            ).ToArray();
        }
        else if (col == 2 && row == 1)
        {
            areas = areas.Where(
                delegate (Core.SpawnPatternArea area)
                {
                    switch (area)
                    {
                        case Core.SpawnPatternArea.L:
                        case Core.SpawnPatternArea.DL:
                        case Core.SpawnPatternArea.D:
                            return false;
                        default:
                            return true;
                    }
                }
            ).ToArray();
        }
        else if (col == 1 && row == 2)
        {
            areas = areas.Where(
                delegate (Core.SpawnPatternArea area)
                {
                    switch (area)
                    {
                        case Core.SpawnPatternArea.U:
                        case Core.SpawnPatternArea.UR:
                        case Core.SpawnPatternArea.R:
                            return false;
                        default:
                            return true;
                    }
                }
            ).ToArray();
        }
        else if (col == 2 && row == 2)
        {
            areas = areas.Where(
                delegate (Core.SpawnPatternArea area)
                {
                    switch (area)
                    {
                        case Core.SpawnPatternArea.U:
                        case Core.SpawnPatternArea.UL:
                        case Core.SpawnPatternArea.L:
                            return false;
                        default:
                            return true;
                    }
                }
            ).ToArray();
        }

        return areas;
    }

    /// <summary>
    /// 今回のスポーンで、指定したアイテムがスポーンした回数を取得する。
    /// </summary>
    /// <param name="type">アイテムタイプ</param>
    /// <returns>スポーン回数</returns>
    private int GetSpawnCountInWave(Core.SpawnType type)
    {
        int count = 0;

        foreach (var result in spawnResults)
        {
            if (result.type == type)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// アイテムオブジェクトを破棄する。
    /// </summary>
    private void DestroySpawnObject()
    {
        if (spawnBase == null)
        {
            return;
        }

        for (int i = 0; i < spawnBase.transform.childCount; i++)
        {
            Transform childTransform = spawnBase.transform.GetChild(i);
            Destroy(childTransform.gameObject);
        }
    }

    /// <summary>
    /// キャラクターの移動速度を加算するかチェックする。
    /// </summary>
    /// <returns>加算する場合はtrueを、そうでない場合はfalseを返す。</returns>
    private bool IsIncreaseCharacterSpeed()
    {
        int perSpwawnCount = CharacterManager.increasePerSpawnCount;
        if (perSpwawnCount > 0)
        {
            if (0 == (spawnCount % perSpwawnCount))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// キャラクターの移動速度を加算する。
    /// </summary>
    private void IncreaseCharacterSpeed()
    {
        CharacterManager.Instance.AddSpeed(CharacterManager.increaseSpeed);
    }

    /// <summary>
    /// スポーンコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator SpawnCoroutine()
    {
        bool isEnd = false;

        while (!isEnd)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (IsPause())
            {
                continue;
            }

            DestroySpawnObject();
            Spawn();

            if (IsIncreaseCharacterSpeed())
            {
                IncreaseCharacterSpeed();
            }
        }
    }

}
