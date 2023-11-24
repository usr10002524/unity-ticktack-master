using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム進行管理クラス
/// </summary>
public class GameController : MonoBehaviour
{
    /// <summary>
    /// ステップ
    /// </summary>
    private enum Step
    {
        // 初期化
        STEP_INIT,
        // フェード街
        STEP_FADE_WAIT,

        // 開始演出開始
        STEP_OPENING_START,
        // 開始演出終了待ち
        STEP_OPENING_WAIT,

        // ゲーム開始
        STEP_GAME_START,
        // ゲーム中
        STEP_GAME_WAIT,

        // ゲームオーバー演出開始
        STEP_GAMEOVER_START,
        // ゲームオーバー演出終了待ち
        STEP_GAMEOVER_WAIT,

        // 結果演出開始
        STEP_RESULT_START,
        // 結果演出終了待ち
        STEP_RESULT_WAIT,

        // 終了
        STEP_END,
    }

    /// <summary>
    /// 現在のステップ
    /// </summary>
    [SerializeField] private Step step = Step.STEP_INIT;

    /// <summary>
    /// スコアを保存するボードID
    /// </summary>
    private static readonly int boardId = 1;


    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        UpdateStep();
    }

    /// <summary>
    /// ステップ処理の更新
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
            case Step.STEP_FADE_WAIT:
                {
                    StepFadeWait();
                    break;
                }

            case Step.STEP_OPENING_START:
                {
                    StepOpeningStart();
                    break;
                }
            case Step.STEP_OPENING_WAIT:
                {
                    StepOpeningWait();
                    break;
                }

            case Step.STEP_GAME_START:
                {
                    StepGameStart();
                    break;
                }
            case Step.STEP_GAME_WAIT:
                {
                    StepGameWait();
                    break;
                }

            case Step.STEP_GAMEOVER_START:
                {
                    StepGameOverStart();
                    break;
                }
            case Step.STEP_GAMEOVER_WAIT:
                {
                    StepGameOverWait();
                    break;
                }

            case Step.STEP_RESULT_START:
                {
                    StepResultStart();
                    break;
                }
            case Step.STEP_RESULT_WAIT:
                {
                    StepResultWait();
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
            // Debug.LogFormat("ChangeStep() {0} -> {1}", step, nextStep);
            step = nextStep;
        }
    }

    /// <summary>
    /// 初期化ステップ
    /// </summary>
    private void StepInit()
    {
        // 各種マネージャの初期化を行う
        TileManager.Instance.Initialize();
        CharacterManager.Instance.Initialize();
        SpawnManager.Instance.Initialize();
        TelopManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
        ScoreManager.Instance.Initialize();
        ReturnTitleManager.Instance.Initialize();
        InstManager.Instance.Initialize();
        SoundVolumeManager.Instance.Initialize();

        BgmManager.Instance.PlayBgm(BgmType.bgm02);
        ChangeStep(Step.STEP_FADE_WAIT);
    }

    /// <summary>
    /// フェード完了待ちステップ
    /// </summary>
    private void StepFadeWait()
    {
        if (Initiate.IsInFadeing())
        {
            return; // フェードが終了するのを待つ
        }
        ChangeStep(Step.STEP_OPENING_START);
    }

    /// <summary>
    /// ゲーム開始演出開始ステップ
    /// </summary>
    private void StepOpeningStart()
    {
        ReadyTelopStart();
        ChangeStep(Step.STEP_OPENING_WAIT);
    }

    /// <summary>
    /// ゲーム開始演出終了待ちステップ
    /// </summary>
    private void StepOpeningWait()
    {
        // 演出側からの終了コールバックで遷移するので、ここでは何もしない
        ;
    }

    /// <summary>
    /// ゲーム開始ステップ
    /// </summary>
    private void StepGameStart()
    {
        CharacterManager.Instance.Run();
        PlayerManager.Instance.Resume();
        SpawnManager.Instance.Resume();
        ScoreManager.Instance.Resume();
        InstManager.Instance.ShowButton(true);
        SoundVolumeManager.Instance.ShowButton(true);
        ChangeStep(Step.STEP_GAME_WAIT);
    }

    /// <summary>
    /// ゲーム中ステップ
    /// </summary>
    private void StepGameWait()
    {
        if (IsMissed())
        {
            // ミスしたらゲーム終了へ
            CharacterManager.Instance.Stop();
            PlayerManager.Instance.Pause();
            SpawnManager.Instance.Pause();
            ScoreManager.Instance.Pause();
            InstManager.Instance.ShowButton(false);
            SoundVolumeManager.Instance.ShowButton(false);
            BgmManager.Instance.FadeStopBgm(0.5f);
            ChangeStep(Step.STEP_GAMEOVER_START);
        }

    }

    /// <summary>
    /// ゲーム終了演出開始ステップ
    /// </summary>
    private void StepGameOverStart()
    {
        SaveData();
        GameOverTelopStart();
        ChangeStep(Step.STEP_GAMEOVER_WAIT);
    }

    /// <summary>
    /// ゲーム終了演出終了待ちステップ
    /// </summary>
    private void StepGameOverWait()
    {
        // 演出側からの終了コールバックで遷移するので、ここでは何もしない
        ;
    }

    /// <summary>
    /// 結果演出開始ステップ
    /// </summary>
    private void StepResultStart()
    {
        ShowScoreBoard();
        ReturnTitleManager.Instance.ShowButton(true);
        ChangeStep(Step.STEP_RESULT_WAIT);
    }

    /// <summary>
    /// 結果演出終了ステップ
    /// </summary>
    private void StepResultWait()
    {
        // タイトルに戻るボタンが押されるのを待つ
        if (ReturnTitleManager.Instance.IsTransitionTitleScene())
        {
            Initiate.Fade("TitleScene", FadeConst.sceneTransitColor, FadeConst.fadeDump, FadeConst.sceneTransitSortOrder);
            ChangeStep(Step.STEP_END);
        }
    }

    /// <summary>
    /// 終了ステップ
    /// </summary>
    private void StepEnd()
    {
        ;
    }

    /// <summary>
    /// ミスしたかどうかチェックする。
    /// </summary>
    /// <returns>ミスした場合はtrueを、そうでない場合はfalseを返す。</returns>
    private bool IsMissed()
    {
        if (CharacterManager.Instance.IsNoPath())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// スコアをサーバに保存する
    /// </summary>
    private void SaveData()
    {
        if (!AtsumaruAPI.Instance.IsValid())
        {
            return; // AtumaruAPIが無効であれば何もしない
        }

        int score = PlayerManager.Instance.GetScore();
        AtsumaruAPI.Instance.SaveScore(boardId, score);
    }

    /// <summary>
    /// スコアボードを表示する
    /// </summary>
    private void ShowScoreBoard()
    {
        if (!AtsumaruAPI.Instance.IsValid())
        {
            return;
        }

        AtsumaruAPI.Instance.DisplayScoreBoard(boardId);
    }

    /// <summary>
    /// 「Ready」テロップを表示する。
    /// </summary>
    private void ReadyTelopStart()
    {
        CharacterManager.Instance.Jump();
        TelopManager.Instance.StartTelop(TelopManager.TelopType.Ready, StartTelopStart);
        SeManager.Instance.PlaySe(SeType.seGameReady);
    }
    /// <summary>
    /// 「Start」テロップを表示する。
    /// 「Ready」テロップ表示終了後のコールバックから呼ばれる。
    /// </summary>
    private void StartTelopStart()
    {
        TelopManager.Instance.StartTelop(TelopManager.TelopType.Start, StartTelopFinishedCallback);
        SeManager.Instance.PlaySe(SeType.seGameStart);

        float delay = SeManager.Instance.GetDuration(SeType.seGameStart);
        CharacterManager.Instance.GameStart(delay);
    }
    /// <summary>
    /// 「Start」テロップの表示終了コールバック。
    /// </summary>
    private void StartTelopFinishedCallback()
    {
        ChangeStep(Step.STEP_GAME_START);
    }

    /// <summary>
    /// 「GameOver」テロップを表示開始する。
    /// </summary>
    private void GameOverTelopStart()
    {
        TelopManager.Instance.StartTelop(TelopManager.TelopType.GameOver, GameOverTelopFinishedCallback);
        SeManager.Instance.PlaySe(SeType.seGameOver);

        float delay = SeManager.Instance.GetDuration(SeType.seGameOver);
        CharacterManager.Instance.GameOver(delay);
    }
    /// <summary>
    /// 「GameOver」テロップの表示終了コールバック。
    /// </summary>
    private void GameOverTelopFinishedCallback()
    {
        ChangeStep(Step.STEP_RESULT_START);
    }
}
