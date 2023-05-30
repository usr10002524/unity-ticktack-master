using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// テロップ表示制御クラス
/// </summary>
public class TelopController : MonoBehaviour
{
    /// <summary>
    /// テロップオブジェクト
    /// </summary>
    [SerializeField] private GameObject targetObject;
    /// <summary>
    /// 表示開始イージングオブジェクト
    /// </summary>
    [SerializeField] private GameObject easingInObject;
    /// <summary>
    /// 表示終了イージングオブジェクト
    /// </summary>
    [SerializeField] private GameObject easingOutObject;

    /// <summary>
    /// 表示開始イージング制御クラス
    /// </summary>
    private EasingController easingInController;
    /// <summary>
    /// 表示終了イージング制御クラス
    /// </summary>
    private EasingController easingOutController;
    /// <summary>
    /// 終了コールバック
    /// </summary>
    private UnityEvent finishCallback;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        UpdateTlop();

        if (IsFinished())
        {
            OnFinished();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初期化処理を行う。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        if (easingInObject != null)
        {
            easingInController = easingInObject.GetComponent<EasingController>();
            easingInController.OnInit(targetObject);
        }
        if (easingOutObject != null)
        {
            easingOutController = easingOutObject.GetComponent<EasingController>();
            easingOutController.OnInit(targetObject);
        }
        // ターゲットは最初は非アクティブにする
        if (targetObject != null)
        {
            targetObject.SetActive(false);
            // Debug.LogFormat("TelopController.Initialize() {0}:SetActive(false).", targetObject);
        }
    }

    /// <summary>
    /// テロップ表示を開始する。
    /// </summary>
    public void StartTelop()
    {
        Initialize();

        if (easingInController != null)
        {
            easingInController.StartEasing();
        }
    }

    /// <summary>
    /// テロップ表示を開始する。
    /// 終了コールバック付き。
    /// </summary>
    /// <param name="callback">終了時にコールバックする呼び先</param>
    public void StartTelop(UnityAction callback)
    {
        if (finishCallback == null)
        {
            finishCallback = new UnityEvent();
            finishCallback.AddListener(callback);
        }

        StartTelop();
    }

    /// <summary>
    /// 表示終了したかチェックする。
    /// </summary>
    /// <returns>表示が終了した場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsFinished()
    {
        if (easingInController != null)
        {
            if (!easingInController.IsFinished())
            {
                return false;
            }
        }
        if (easingOutController != null)
        {
            if (!easingOutController.IsFinished())
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// テロップ表示の更新処理
    /// </summary>
    private void UpdateTlop()
    {
        if (easingInController != null)
        {
            if (easingInController.IsFinished())
            {
                if (easingOutController != null)
                {
                    if (!easingOutController.IsStarted())
                    {
                        easingOutController.StartEasing();
                    }
                }
            }
        }
        else
        {
            if (easingOutController != null)
            {
                if (!easingOutController.IsStarted())
                {
                    easingOutController.StartEasing();
                }
            }
        }
    }

    /// <summary>
    /// 表示終了時のコールバックを呼ぶ
    /// </summary>
    private void OnFinished()
    {
        if (finishCallback != null)
        {
            finishCallback.Invoke();
        }
    }

}
