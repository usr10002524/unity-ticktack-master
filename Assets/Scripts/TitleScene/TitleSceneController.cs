using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// タイトルシーン制御クラス
/// </summary>
public class TitleSceneController : MonoBehaviour
{
    /// <summary>
    /// ステップ
    /// </summary>
    private enum Step
    {
        // 初期化
        STEP_INIT,

        // フェード待ち
        STEP_FADE_WAIT,

        // タイトル表示開始
        STEP_TITLE_START,
        // タイトル表示終了待ち
        STEP_TITLE_WAIT,

        // 終了
        STEP_END,
    }

    /// <summary>
    /// 現在のステップ
    /// </summary>
    [SerializeField] Step step;

    /// <summary>
    /// 前回のタイル座標
    /// </summary>
    private Vector2Int lastPosition;

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
    /// ステップの更新処理
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

            case Step.STEP_TITLE_START:
                {
                    StepTitleStart();
                    break;
                }
            case Step.STEP_TITLE_WAIT:
                {
                    StepTitleWait();
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
            // Debug.LogFormat("TitleSceneController.ChangeStep() {0} -> {1}", step, nextStep);
            step = nextStep;
        }
    }

    /// <summary>
    /// 初期化ステップ
    /// </summary>
    private void StepInit()
    {
        TileManager.Instance.InitializeTitleScene();
        CharacterManager.Instance.InitializeTitleScene();
        TitleMaanger.Instance.Initialize();
        InstManager.Instance.InitializeTitleScene();
        SoundVolumeManager.Instance.InitializeTitleScene();
        BgmManager.Instance.PlayBgm(BgmType.bgm01);

        lastPosition = CharacterManager.Instance.GetPosition();
        ChangeStep(Step.STEP_FADE_WAIT);
    }

    /// <summary>
    /// フェード待ちステップ
    /// </summary>
    private void StepFadeWait()
    {
        if (Initiate.IsInFadeing())
        {
            return;
        }
        ChangeStep(Step.STEP_TITLE_START);
    }

    /// <summary>
    /// タイトル表示開始ステップ
    /// </summary>
    private void StepTitleStart()
    {
        CharacterManager.Instance.Run();
        ChangeStep(Step.STEP_TITLE_WAIT);
    }

    /// <summary>
    /// タイトル表示中ステップ
    /// </summary>
    private void StepTitleWait()
    {
        UpdateTitleMain();

        if (TitleMaanger.Instance.IsTransitionMainScene())
        {
            BgmManager.Instance.FadeStopBgm(0.5f);
            NextScene();
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
    /// 次のシーンへ遷移する
    /// </summary>
    private void NextScene()
    {
        Initiate.Fade("MainScene", FadeConst.sceneTransitColor, FadeConst.fadeDump, FadeConst.sceneTransitSortOrder);
    }

    /// <summary>
    /// デモ中のタイル移動処理
    /// </summary>
    private void UpdateTitleMain()
    {
        CheckTileMove();
    }

    /// <summary>
    /// デモ中のタイルを移動させる処理
    /// </summary>
    private void CheckTileMove()
    {
        Vector2Int pos = CharacterManager.Instance.GetPosition();

        if (lastPosition != pos)
        {
            lastPosition = pos;

            if (pos.x == 0 && pos.y == 2)
            {
                TileManager.Instance.MoveSlide(1, 0);
                // TileManager.Instance.Slide(1, 0);
            }
            else if (pos.x == 2 && pos.y == 0)
            {
                TileManager.Instance.MoveSlide(1, 2);
                // TileManager.Instance.Slide(1, 2);
            }
        }
    }
}
