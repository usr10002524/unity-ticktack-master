using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// テロップ表示管理クラス
/// </summary>
public class TelopManager : MonoBehaviour
{
    /// <summary>
    /// テロップタイプ
    /// </summary>
    public enum TelopType
    {
        Ready,
        Start,
        GameOver,
    }

    /// <summary>
    /// テロップ情報
    /// </summary>
    [System.Serializable]
    public class TelopInfo
    {
        /// <summary>
        /// テロップタイプ
        /// </summary>
        public TelopType type;
        /// <summary>
        /// テロップのプレファブ
        /// </summary>
        public GameObject telopPrefab;
    }

    /// <summary>
    /// テロップ情報リスト
    /// </summary>
    /// <value></value>
    [SerializeField] private List<TelopInfo> telopInfos;
    /// <summary>
    /// テロップを表示する際の親オブジェクト
    /// </summary>
    /// <value></value>
    [SerializeField] private GameObject telopBase;

    /// <summary>
    /// シングルトンのインスタンス。
    /// </summary>
    public static TelopManager Instance { get; private set; }

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
    /// テロップの表示を行う。
    /// </summary>
    /// <param name="type">テロップタイプ</param>
    /// <param name="finishCallback">表示終了後のコールバック</param>
    public void StartTelop(TelopType type, UnityAction finishCallback)
    {
        // 指定されたタイプのテロッププレファブを取得
        GameObject telopPrefab = FindPrefab(type);
        if (telopPrefab == null)
        {
            return;
        }

        // インスタンス化
        GameObject telopObject = Instantiate(telopPrefab, telopBase.transform);
        if (telopObject == null)
        {
            return;
        }

        // テロップ開始
        TelopController telopController = telopObject.GetComponent<TelopController>();
        if (telopController == null)
        {
            return;
        }
        telopController.StartTelop(finishCallback);
    }

    /// <summary>
    /// してしたタイプのプレファブを検索する
    /// </summary>
    /// <param name="type">テロップタイプ</param>
    /// <returns>テロップオブジェクトのプレファブ</returns>
    private GameObject FindPrefab(TelopType type)
    {
        TelopInfo telopInfo = telopInfos.Find(
            delegate (TelopInfo info)
            {
                return (info.type == type);
            }
        );

        if (telopInfo == null)
        {
            return null;
        }
        return telopInfo.telopPrefab;
    }

    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {

    }
}
