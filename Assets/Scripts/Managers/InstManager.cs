using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ヘルプ管理クラス
/// </summary>
public class InstManager : MonoBehaviour
{
    /// <summary>
    /// ヘルプゲームオブジェクトのプレファブ
    /// </summary>
    [SerializeField] private GameObject instPrefab;
    /// <summary>
    /// ヘルプゲームオブジェクト作成時の親
    /// </summary>
    [SerializeField] private GameObject instBase;

    /// <summary>
    /// ヘルプゲームオブジェクト
    /// </summary>
    private GameObject instObject;
    /// <summary>
    /// ヘルプ制御クラス
    /// </summary>
    private InstController instController;

    /// <summary>
    /// タイムスケール
    /// </summary>
    private float timeScale;

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    public static InstManager Instance { get; private set; }


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
        if (instPrefab == null)
        {
            return;
        }
        if (instBase == null)
        {
            return;
        }

        // ヘルプゲームオブジェクトを生成する
        instObject = Instantiate(instPrefab, instBase.transform);
        if (instObject == null)
        {
            return;
        }
        // ヘルプ制御クラスを取得し、初期化を行う
        instController = instObject.GetComponent<InstController>();
        instController.Initialize();
        instController.SetOpenCallback(OnOpenCallback);
        instController.SetCloseCallback(OnCloseCallback);
        // ボタンは非表示にしておく
        ShowButton(false);
    }

    /// <summary>
    /// タイトルシーン用の初期化処理を行う
    /// </summary>
    public void InitializeTitleScene()
    {
        if (instPrefab == null)
        {
            return;
        }
        if (instBase == null)
        {
            return;
        }

        instObject = Instantiate(instPrefab, instBase.transform);
        if (instObject == null)
        {
            return;
        }
        instController = instObject.GetComponent<InstController>();
        instController.Initialize();
    }

    /// <summary>
    /// ヘルプボタンのトグル処理
    /// </summary>
    public void OnToggle()
    {
        if (instController == null)
        {
            return;
        }
        instController.OnToggleButton();
    }

    /// <summary>
    /// ヘルプを閉じるときの処理
    /// </summary>
    public void OnClose()
    {
        if (instController == null)
        {
            return;
        }
        instController.OnClickCloseButton();
    }

    /// <summary>
    /// ヘルプのページ送り処理
    /// </summary>
    public void OnNext()
    {
        if (instController == null)
        {
            return;
        }
        instController.OnClickNextButton();
    }

    /// <summary>
    /// ヘルプのページ戻し処理
    /// </summary>
    public void OnPrev()
    {
        if (instController == null)
        {
            return;
        }
        instController.OnClickPrevButton();
    }

    /// <summary>
    /// へるぷぼたんの表示、非表示を行う。
    /// </summary>
    /// <param name="flag">表示するときはtrueを、非表示にするときはfalseを指定する。</param>
    public void ShowButton(bool flag)
    {
        if (instBase == null)
        {
            return;
        }
        instBase.SetActive(flag);
    }

    /// <summary>
    /// ヘルプを開いたときのコールバック処理を行う。
    /// </summary>
    private void OnOpenCallback()
    {
        CharacterManager.Instance.Stop();
    }

    /// <summary>
    /// ヘルプを閉じたときのコールバックを処理を行う。
    /// </summary>
    private void OnCloseCallback()
    {
        if (!CharacterManager.Instance.IsNoPath())
        {
            CharacterManager.Instance.Run();
        }
    }
}
