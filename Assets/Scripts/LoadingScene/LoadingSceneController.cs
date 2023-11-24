using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ロードシーン制御クラス
/// </summary>
public class LoadingSceneController : MonoBehaviour
{
    /// <summary>
    /// ステップ
    /// </summary>
    private enum Step
    {
        // 初期化ステップ
        STEP_INIT,

        // ロード開始ステップ
        STEP_LOAD_START,
        // ロード中ステップ
        STEP_LOAD_WAIT,
        // ロード完了ステップ
        STEP_COMPLETE,
        // ロードタイムアウトステップ
        STEP_TIMEOUT,

        // ロード終了開始ステップ
        STEP_FINISH_START,
        // ロード終了待ちステップ
        STEP_FINISH_WAIT,

        // 終了ステップ
        STEP_END,
    }

    /// <summary>
    /// 現在のステップ
    /// </summary>
    [SerializeField] private Step step;

    /// <summary>
    /// タイムアウトタイマー
    /// </summary>
    private float timeoutTimer;

    /// <summary>
    /// タイムアウト時間
    /// </summary>
    private static readonly float TimeoutTime = 60.0f;


    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        step = Step.STEP_INIT;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        UpdateStep();
    }

    /// <summary>
    /// ステップ更新処理を行う。
    /// </summary>
    private void UpdateStep()
    {
        switch (step)
        {
            case Step.STEP_INIT:
                {
                    StepInit();
                    break;
                }

            case Step.STEP_LOAD_START:
                {
                    StepLoadStart();
                    break;
                }
            case Step.STEP_LOAD_WAIT:
                {
                    StepLoadWait();
                    break;
                }
            case Step.STEP_COMPLETE:
                {
                    StepComplete();
                    break;
                }
            case Step.STEP_TIMEOUT:
                {
                    StepTimeout();
                    break;
                }

            case Step.STEP_FINISH_START:
                {
                    StepFinishStart();
                    break;
                }
            case Step.STEP_FINISH_WAIT:
                {
                    StepFinishWait();
                    break;
                }

            case Step.STEP_END:
                {
                    StepEnd();
                    break;
                }
        }
    }

    /// <summary>
    /// ステップの変更を行う
    /// </summary>
    /// <param name="nextStep">変更先のステップ</param>
    private void ChangeStep(Step nextStep)
    {
        if (step != nextStep)
        {
            // Debug.LogFormat("LoadingSceneController.ChangeStep() {0} -> {1}", step, nextStep);
            step = nextStep;
        }
    }

    /// <summary>
    /// 初期化ステップ
    /// </summary>
    private void StepInit()
    {
        // // AtsumaruAPIが無効の場合は即終了
        // if (!AtsumaruAPI.Instance.IsValid())
        // {
        //     ChangeStep(Step.STEP_END);
        // }
        // else
        // {
        //     LoadingManager.Instance.Initialize();
        //     ChangeStep(Step.STEP_LOAD_START);
        // }

        // AtsumaruAPI無効のときはLocalStorageに切り替えるので、とりあえずロードはスタートさせる
        LoadingManager.Instance.Initialize();
        ChangeStep(Step.STEP_LOAD_START);
    }

    /// <summary>
    /// ロード開始ステップ
    /// </summary>
    private void StepLoadStart()
    {
        LoadingManager.Instance.StartLoading();
        if (AtsumaruAPI.Instance.IsValid())
        {
            AtsumaruAPI.Instance.LoadServerData();
        }
        else
        {
            LocalStorageAPI.Instance.LoadLocalData();
        }
        ChangeStep(Step.STEP_LOAD_WAIT);
    }

    /// <summary>
    /// ロード中ステップ
    /// </summary>
    private void StepLoadWait()
    {
        timeoutTimer += Time.deltaTime;
        if (IsDataLoaded())
        {
            // ロード完了
            ChangeStep(Step.STEP_COMPLETE);
        }
        else if (timeoutTimer >= TimeoutTime)
        {
            // ロードタイムアウト
            ChangeStep(Step.STEP_TIMEOUT);
        }
        else
        {
            ;
        }
    }

    /// <summary>
    /// データロードが完了しているか確認する
    /// </summary>
    /// <returns>完了している場合はtrue、そうでない場合はfalseを返す</returns>
    private bool IsDataLoaded()
    {
        if (AtsumaruAPI.Instance.IsValid())
        {
            return AtsumaruAPI.Instance.IsServerDataLoaded();
        }
        else
        {
            return LocalStorageAPI.Instance.IsLocalDataLoaded();
        }
    }

    /// <summary>
    /// ロード完了ステップ
    /// </summary>
    private void StepComplete()
    {
        LoadingManager.Instance.SetComplete();
        ChangeStep(Step.STEP_FINISH_START);
    }

    /// <summary>
    /// ロードタイムアウトステップ
    /// </summary>
    private void StepTimeout()
    {
        LoadingManager.Instance.SetTimeout();
        ChangeStep(Step.STEP_FINISH_START);
    }

    /// <summary>
    /// ロード終了ステップ
    /// </summary>
    private void StepFinishStart()
    {
        ChangeStep(Step.STEP_FINISH_WAIT);
    }

    /// <summary>
    /// ロード終了待ちステップ
    /// </summary>
    private void StepFinishWait()
    {
        if (!LoadingManager.Instance.IsFinished())
        {
            return;
        }
        ChangeStep(Step.STEP_END);
    }

    /// <summary>
    /// 終了ステップ
    /// </summary>
    private void StepEnd()
    {
        NextScene();
    }

    /// <summary>
    /// 次のシーンへ遷移する
    /// </summary>
    private void NextScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
