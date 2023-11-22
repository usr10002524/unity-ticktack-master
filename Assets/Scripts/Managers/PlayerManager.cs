using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// プレーヤーマネージャ
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [SerializeField] int score;
    [SerializeField] float rate;

    private bool isPause;

    private static readonly float MaxRate = 5.0f;
    private static readonly int MaxLevel = 5;

    public static PlayerManager Instance { get; private set; }


    /// <summary>
    /// 初期化を行う
    /// </summary>
    public void Initialize()
    {
        score = 0;
        rate = 0.0f;
        isPause = true;
    }

    /// <summary>
    /// ポーズ中かどうかをチェックする。
    /// </summary>
    /// <returns>ポーズ中の場合はtrueを返す。そうでない場合はfalseを返す。</returns>
    public bool IsPause()
    {
        return isPause;
    }

    /// <summary>
    /// 入力受付を中断する
    /// </summary>
    public void Pause()
    {
        isPause = true;
    }

    /// <summary>
    /// 入力受付を再開する
    /// </summary>
    public void Resume()
    {
        isPause = false;
    }

    /// <summary>
    /// スコアを加算する
    /// </summary>
    /// <param name="add">加算する値</param>
    public void AddScore(int add)
    {
        score += add;
    }

    /// <summary>
    /// スコアをクリアする。
    /// </summary>
    public void ClearScore()
    {
        score = 0;
    }

    /// <summary>
    /// スコアを取得する
    /// </summary>
    /// <returns>現在のスコア</returns>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// スコアレートを加算する。
    /// </summary>
    /// <param name="add">加算する値</param>
    public void AddRate(float add)
    {
        rate += add;

        rate = Mathf.Clamp(rate, 0.0f, MaxRate);
    }

    /// <summary>
    /// 0.0～1.0fに丸めたスコアレートを取得する
    /// </summary>
    /// <returns>0.0～1.0fに丸めたスコアレート</returns>
    public float GetClampRate()
    {
        float clampRate = 0.0f;
        if (rate < MaxRate)
        {
            clampRate = rate % 1.0f;  // 0.0 <= clampRate < 1.0 に丸める
        }
        else
        {
            clampRate = 1.0f;
        }
        return clampRate;
    }

    /// <summary>
    /// スコアレートのレベルを取得する
    /// </summary>
    /// <returns>スコアレートのレベル</returns>
    public int GetRateLevel()
    {
        int rateLevel = (int)rate;
        rateLevel += 1;
        rateLevel = Mathf.Clamp(rateLevel, 1, MaxLevel);
        return rateLevel;
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
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        UpdateInput();
    }

    /// <summary>
    /// 入力処理の更新
    /// </summary>
    private void UpdateInput()
    {
        if (IsPause())
        {
            return;
        }

        string[] targetLayer = {
            "Tiles",
        };
        int layerMask = LayerMask.GetMask(targetLayer);
        float maxDistance = 100.0f;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit = new RaycastHit();

            if (Physics.Raycast(ray, out raycastHit, maxDistance, layerMask))
            {
                GameObject hittedObject = raycastHit.collider.gameObject;
                OnTileClick(hittedObject);
            }
        }
    }

    /// <summary>
    /// タイルをクリックした際の処理
    /// </summary>
    /// <param name="obj">クリックしたオブジェクト</param>
    private void OnTileClick(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        if (obj.transform.parent == null)
        {
            return;
        }
        if (TileManager.Instance.IsInEasing())
        {
            return;
        }

        GameObject parent = obj.transform.parent.gameObject;
        if (parent == null)
        {
            return;
        }

        TileController tileController = parent.GetComponent<TileController>();
        if (tileController == null)
        {
            return;
        }

        SeManager.Instance.PlaySe(SeType.seClick);

        Vector2Int pos = tileController.GetPosition();
        TileManager.Instance.MoveSlide(pos.x, pos.y);
    }
}
