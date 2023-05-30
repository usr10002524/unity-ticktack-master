using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター管理クラス
/// </summary>
public class CharacterManager : MonoBehaviour
{
    /// <summary>
    /// キャラクターゲームオブジェクト
    /// </summary>
    [SerializeField] GameObject characterObject;

    /// <summary>
    /// 移動速度
    /// </summary>
    private float speed;
    /// <summary>
    /// パス移動オブジェクト
    /// </summary>
    private MoveByPath mover;
    /// <summary>
    /// キャラクター制御クラス
    /// </summary>
    private CharacterController controller;

    /// <summary>
    /// 初期スピード
    /// </summary>
    private static readonly float initialiSpeed = 0.2f;
    /// <summary>
    /// 最小スピード
    /// </summary>
    private static readonly float minSpeed = 0.01f;
    /// <summary>
    /// 最大スピード
    /// </summary>
    private static readonly float maxSpeed = 1.0f;

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    public static CharacterManager Instance { get; private set; }

    /// <summary>
    /// 加算スピード
    /// </summary>
    public static readonly float increaseSpeed = 0.05f;
    /// <summary>
    /// スピードを加算するスポーン回数
    /// </summary>
    public static readonly int increasePerSpawnCount = 3;


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
    /// 初期化処理。
    /// ゲーム開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        if (characterObject == null)
        {
            return;
        }

        // MoveByPathクラスオブジェクトを取得
        mover = characterObject.GetComponent<MoveByPath>();
        if (mover == null)
        {
            return;
        }

        // CharacterControllerクラスオブジェクトを取得
        controller = characterObject.GetComponent<CharacterController>();
        if (controller == null)
        {
            return;
        }

        // 初期速度を設定
        speed = initialiSpeed;
        // MoveByPathを初期化
        mover.Initialize(new Vector2Int(1, 1));
        // CharacterControllerを初期化
        controller.Initialize();
    }

    /// <summary>
    /// タイトルシーンでの初期化処理を行う。
    /// </summary>
    public void InitializeTitleScene()
    {
        if (characterObject == null)
        {
            return;
        }

        mover = characterObject.GetComponent<MoveByPath>();
        if (mover == null)
        {
            return;
        }

        controller = characterObject.GetComponent<CharacterController>();
        if (controller == null)
        {
            return;
        }

        speed = initialiSpeed;
        mover.Initialize(new Vector2Int(1, 1));
        controller.Initialize();
    }

    /// <summary>
    /// キャラクターの移動を止める。
    /// </summary>
    public void Stop()
    {
        if (mover == null)
        {
            return;
        }
        mover.SetSpeed(0);
    }

    /// <summary>
    /// キャラクターを移動させる。
    /// </summary>
    public void Run()
    {
        if (mover == null)
        {
            return;
        }
        mover.SetSpeed(speed);
    }

    /// <summary>
    /// キャラクターをジャンプさせる。
    /// </summary>
    public void Jump()
    {
        if (controller == null)
        {
            return;
        }
        controller.JumpMotion();
    }

    /// <summary>
    /// キャラクターが移動中かチェックする。
    /// </summary>
    /// <returns>移動中の場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsRun()
    {
        if (mover == null)
        {
            return false;
        }
        else
        {
            if (mover.GetSpeed() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// キャラクターが停止中かチェックする。
    /// </summary>
    /// <returns>停止中の場合はtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsStop()
    {
        return (!IsRun());
    }

    /// <summary>
    /// 移動速度を加算する
    /// </summary>
    /// <param name="add">加算する値</param>
    public void AddSpeed(float add)
    {
        speed += add;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        if (mover == null)
        {
            return;
        }
        mover.SetSpeed(speed);
    }

    /// <summary>
    /// 移動先パスがない状態かどうかをチェックする。
    /// </summary>
    /// <returns>移動先パスがない状態のときはtrueを、そうでない場合はfalseを返す。</returns>
    public bool IsNoPath()
    {
        if (mover == null)
        {
            return false;
        }

        return mover.IsNoPath();
    }

    /// <summary>
    /// 現在のタイル座標を取得する。
    /// </summary>
    /// <returns>現在のタイル座標</returns>
    public Vector2Int GetPosition()
    {
        Vector2Int pos = new Vector2Int();
        if (controller == null)
        {
            return pos;
        }

        return controller.GetPosition();
    }

    /// <summary>
    /// ゲーム開始時の処理を行う
    /// </summary>
    /// <param name="delay">ボイス再生タイミング</param>
    public void GameStart(float delay)
    {
        VoiceType[] types ={
            VoiceType.voGameStart01,
            VoiceType.voGameStart02,
            VoiceType.voGameStart03,
        };

        int index = Random.Range(0, types.Length);
        VoiceManager.Instance.PlayVoice(types[index], delay);
    }

    /// <summary>
    /// ゲームオーバー時の処理を行う。
    /// </summary>
    /// <param name="delay">ボイス再生タイミング</param>
    public void GameOver(float delay)
    {
        VoiceType[] types ={
            VoiceType.voGameOver01,
            VoiceType.voGameOver02,
            VoiceType.voGameOver03,
            VoiceType.voGameOver04,
        };

        int index = Random.Range(0, types.Length);
        VoiceManager.Instance.PlayVoice(types[index], delay);
    }

    /// <summary>
    /// スコア獲得時の処理を行う。
    /// </summary>
    /// <param name="delay">ボイス再生タイミング</param>
    public void ScoreGet(float delay)
    {
        VoiceType[] types ={
            VoiceType.voScoreGet01,
            VoiceType.voScoreGet02,
            VoiceType.voScoreGet03,
        };

        int index = Random.Range(0, types.Length);
        VoiceManager.Instance.PlayVoice(types[index], delay);
    }
}
