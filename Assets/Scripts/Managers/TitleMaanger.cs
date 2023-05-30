using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトル表示管理クラス
/// </summary>
public class TitleMaanger : MonoBehaviour
{
    /// <summary>
    /// タイトル表示オブジェクトのプレファブ
    /// </summary>
    [SerializeField] private GameObject titlePrefab;
    /// <summary>
    /// タイトル表示オブジェクトの親オブジェクト
    /// </summary>
    [SerializeField] private GameObject titleBase;

    /// <summary>
    /// タイトル表示オブジェクト
    /// </summary>
    private GameObject titleObject;
    /// <summary>
    /// タイトル表示制御クラス
    /// </summary>
    private TitleController titleController;

    /// <summary>
    /// シングルトンのインスタンス。
    /// </summary>
    public static TitleMaanger Instance { get; private set; }

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
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        if (titlePrefab == null)
        {
            return;
        }
        if (titleBase == null)
        {
            return;
        }

        titleObject = Instantiate(titlePrefab, titleBase.transform);
        if (titleObject == null)
        {
            return;
        }

        titleController = titleObject.GetComponent<TitleController>();
        if (titleController == null)
        {
            return;
        }
        titleController.Initialize();
    }

    /// <summary>
    /// メインシーンに遷移中かどうかチェックする。
    /// </summary>
    /// <returns>遷移中の場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsTransitionMainScene()
    {
        if (titleController == null)
        {
            return true;
        }
        return titleController.IsTransitionMainScene();
    }
}
