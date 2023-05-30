using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// ヘルプ表示制御クラス
/// </summary>
public class InstController : MonoBehaviour
{
    /// <summary>
    /// ヘルプウィンドウのステータス
    /// </summary>
    public enum Stat
    {
        // 閉状態
        Close,
        // 開状態
        Open,
    };

    /// <summary>
    /// 説明スプライトのリスト
    /// </summary>
    [SerializeField] private List<Sprite> instSprites;
    /// <summary>
    /// イメージオブジェクト
    /// </summary>
    [SerializeField] private Image instImage;
    /// <summary>
    /// コンテナオブジェクト
    /// </summary>
    [SerializeField] private GameObject containerObject;
    /// <summary>
    /// ヘルプウィンドウのウラに表示するシェード用オブジェクト
    /// </summary>
    [SerializeField] private GameObject shadeObject;
    /// <summary>
    /// ウィンドウを開くときのイージングオブジェクト
    /// </summary>
    [SerializeField] private GameObject easingOpenObject;
    /// <summary>
    /// ウィンドウを閉じるときのイージングオブジェクト
    /// </summary>
    [SerializeField] private GameObject easingCloseObject;

    /// <summary>
    /// 現在のページ
    /// </summary>
    private int currentPage;
    /// <summary>
    /// 現在の状態
    /// </summary>
    private Stat stat;
    /// <summary>
    /// ウィンドウを開くときのイージング制御クラス
    /// </summary>
    private EasingController easingOpenController;
    /// <summary>
    /// ウィンドウを閉じるときのイージング制御クラス
    /// </summary>
    private EasingController easingCloseController;

    /// <summary>
    /// ウィンドウを開いたあとのコールバック
    /// </summary>
    private UnityEvent onOpenCallback;
    /// <summary>
    /// ウィンドウを閉じたあとのコールバック
    /// </summary>
    private UnityEvent onCloseCallback;

    /// <summary>
    /// イージングコルーチン
    /// </summary>
    private Coroutine easingCoroutine;

    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        currentPage = 0;
        stat = Stat.Close;

        InitEasing();
        SetPage(currentPage);
        SetShadeActive(false);
    }

    /// <summary>
    /// 現在のページを設定
    /// </summary>
    /// <param name="page">現在のページ</param>
    public void SetPage(int page)
    {
        if (instImage == null)
        {
            return;
        }
        if (page < 0 || page >= GetPageMax())
        {
            return;
        }

        instImage.sprite = instSprites[page];
        currentPage = page;
    }

    /// <summary>
    /// 現在のページを取得
    /// </summary>
    /// <returns>現在のページ</returns>
    public int GetCurrentPage()
    {
        return currentPage;
    }

    /// <summary>
    /// ページ最大数を取得
    /// </summary>
    /// <returns>ページ最大数</returns>
    public int GetPageMax()
    {
        return instSprites.Count;
    }

    /// <summary>
    /// ページ戻しボタンをクリックしたときの処理
    /// </summary>
    public void OnClickPrevButton()
    {
        if (stat != Stat.Open)
        {
            return;
        }

        if (currentPage > 0)
        {
            currentPage--;
        }
        SetPage(currentPage);
        SeManager.Instance.PlaySe(SeType.seSelect);
    }

    /// <summary>
    /// ページ送りボタンをクリックしたときの処理
    /// </summary>
    public void OnClickNextButton()
    {
        if (stat != Stat.Open)
        {
            return;
        }

        if (currentPage < GetPageMax() - 1)
        {
            currentPage++;
        }
        SetPage(currentPage);
        SeManager.Instance.PlaySe(SeType.seSelect);
    }

    /// <summary>
    /// 閉じるボタンをクリックしたときの処理
    /// </summary>
    public void OnClickCloseButton()
    {
        if (stat != Stat.Open)
        {
            return;
        }

        if (IsInEasing())
        {
            return;
        }

        CloseInstraction();
        SeManager.Instance.PlaySe(SeType.seSelect);
    }

    /// <summary>
    /// ウィンドウ開閉のトグル処理
    /// </summary>
    public void OnToggleButton()
    {
        if (IsInEasing())
        {
            return;
        }

        if (stat == Stat.Open)
        {
            CloseInstraction();
        }
        else
        {
            OpenInstraction();
        }
        SeManager.Instance.PlaySe(SeType.seDecide);
    }

    /// <summary>
    /// ウィンドウ開閉のアニメーション中かどうかをチェックする。
    /// </summary>
    /// <returns>開閉のアニメーション中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsInEasing()
    {
        return (easingCoroutine != null);
    }

    /// <summary>
    /// ウィンドウが開いたときのコールバックを設定する。
    /// </summary>
    /// <param name="action">コールバックの際呼ばれる処理</param>
    public void SetOpenCallback(UnityAction action)
    {
        if (onOpenCallback == null)
        {
            onOpenCallback = new UnityEvent();
        }
        if (onOpenCallback != null)
        {
            onOpenCallback.AddListener(action);
        }
    }

    /// <summary>
    /// ウィンドウが閉じたときのコールバックを設定する。
    /// </summary>
    /// <param name="action">コールバックの際呼ばれる処理</param>
    public void SetCloseCallback(UnityAction action)
    {
        if (onCloseCallback == null)
        {
            onCloseCallback = new UnityEvent();
        }
        if (onCloseCallback != null)
        {
            onCloseCallback.AddListener(action);
        }
    }



    /// <summary>
    /// ウィンドウを開く
    /// </summary>
    private void OpenInstraction()
    {
        StartOpenCoroutine();
        SetShadeActive(true);
        OnOpenCallback();
    }

    /// <summary>
    /// ウィンドウを閉じる
    /// </summary>
    private void CloseInstraction()
    {
        StartCloseCoroutine();
    }



    /// <summary>
    /// イージングの初期化
    /// </summary>
    private void InitEasing()
    {
        if (easingOpenObject != null)
        {
            easingOpenController = easingOpenObject.GetComponent<EasingController>();
            easingOpenController.OnInit(containerObject);
        }
        if (easingCloseObject != null)
        {
            easingCloseController = easingCloseObject.GetComponent<EasingController>();
            easingCloseController.OnInit(containerObject);
        }
    }

    /// <summary>
    /// ウィンドウ開くイージングを開始する。
    /// </summary>
    private void OpenEasing()
    {
        if (easingOpenController == null)
        {
            return;
        }
        easingOpenController.StartEasing();
    }

    /// <summary>
    /// ウィンドウ開くイージングが終了しているかチェックする。
    /// </summary>
    /// <returns>イージングが終了している場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool IsOpenEasingFnished()
    {
        if (easingOpenController == null)
        {
            return true;
        }
        return easingOpenController.IsFinished();
    }

    /// <summary>
    /// ウィンドウが閉じるイージングを開始する。
    /// </summary>
    private void CloseEasing()
    {
        if (easingCloseController == null)
        {
            return;
        }
        easingCloseController.StartEasing();
    }

    /// <summary>
    /// ウィンドウ閉じるイージングが終了しているかチェックする。
    /// </summary>
    /// <returns>イージングが終了している場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool IsCloseEasingFnished()
    {
        if (easingCloseController == null)
        {
            return true;
        }
        return easingCloseController.IsFinished();
    }

    /// <summary>
    /// ウィンドウが開くコルーチンを開始する。
    /// </summary>
    private void StartOpenCoroutine()
    {
        StopOpenCoroutine();
        easingCoroutine = StartCoroutine("OpenCoroutine");

    }

    /// <summary>
    /// ウィンドウが開くコルーチンを停止する。
    /// </summary>
    private void StopOpenCoroutine()
    {
        if (easingCoroutine != null)
        {
            StopCoroutine(easingCoroutine);
            easingCoroutine = null;
            stat = Stat.Open;
        }
    }

    /// <summary>
    /// ウィンドウが閉じるコルーチンを開始する。
    /// </summary>
    private void StartCloseCoroutine()
    {
        StopCloseCoroutine();
        easingCoroutine = StartCoroutine("CloseCoroutine");
        OnCloseCallback();
    }

    /// <summary>
    /// ウィンドウが閉じるコルーチンを停止する。
    /// </summary>
    private void StopCloseCoroutine()
    {
        if (easingCoroutine != null)
        {
            StopCoroutine(easingCoroutine);
            easingCoroutine = null;
            stat = Stat.Close;
        }
        SetShadeActive(false);
    }


    /// <summary>
    /// ウィンドウが開くコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator OpenCoroutine()
    {
        OpenEasing();

        while (!IsOpenEasingFnished())
        {
            yield return null;
        }

        stat = Stat.Open;
        easingCoroutine = null;
    }

    /// <summary>
    /// ウィンドウが閉じるコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator CloseCoroutine()
    {
        CloseEasing();

        while (!IsCloseEasingFnished())
        {
            yield return null;
        }

        stat = Stat.Close;
        easingCoroutine = null;
        StopCloseCoroutine();

    }

    /// <summary>
    /// ヘルプウィンドウ裏のシェードの表示、非表示を設定する。
    /// </summary>
    /// <param name="flag">trueの場合は表示、falseの場合は非表示</param>
    private void SetShadeActive(bool flag)
    {
        if (shadeObject == null)
        {
            return;
        }
        shadeObject.SetActive(flag);
    }

    /// <summary>
    /// ウィンドウが開いたときのコールバックを呼ぶ。
    /// </summary>
    private void OnOpenCallback()
    {
        if (onOpenCallback != null)
        {
            onOpenCallback.Invoke();
        }
    }

    /// <summary>
    /// ウィンドウが閉じたときのコールバックを呼ぶ。
    /// </summary>
    private void OnCloseCallback()
    {
        if (onCloseCallback != null)
        {
            onCloseCallback.Invoke();
        }
    }
}
