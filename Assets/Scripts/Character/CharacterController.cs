using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターコントローラ
/// </summary>
public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// キャラクターモデルのプレファブ
    /// </summary>
    [SerializeField] private GameObject characterPrefav;

    /// <summary>
    /// キャラクターのゲームオブエクと
    /// </summary>
    private GameObject characterObject;
    /// <summary>
    /// キャラクターのアニメータ
    /// </summary>
    private Animator animator;
    /// <summary>
    /// キャラクターに設定されているパス移動情報
    /// </summary>
    private MoveByPath mover;

    /// <summary>
    /// アニメータのパラメータ。
    /// 速度。
    /// </summary>
    private static readonly string SpeedParamName = "Speed";
    /// <summary>
    /// アニメータのパラメータ。
    /// ジャンプトリガー。
    /// </summary>
    private static readonly string JumpParamName = "JumpTrigger";

    /// <summary>
    /// アイテムとコリジョンを取る際のタグ。
    /// </summary>
    private static readonly string TagItems = "Items";

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        UpdateAnimator();
    }

    /// <summary>
    /// 初期化を行う。
    /// シーン開始時に一度呼び出す。
    /// </summary>
    public void Initialize()
    {
        if (characterObject != null)
        {
            Destroy(characterObject);
        }

        if (characterPrefav != null)
        {
            characterObject = Instantiate(characterPrefav, transform);
        }

        if (characterObject != null)
        {
            animator = characterObject.GetComponent<Animator>();
        }

        mover = GetComponent<MoveByPath>();
    }

    /// <summary>
    /// キャラクターのジャンプアニメを再生する。
    /// </summary>
    public void JumpMotion()
    {
        if (animator != null)
        {
            animator.SetTrigger(JumpParamName);
        }
    }

    /// <summary>
    /// 現在キャラクターが乗っているタイルの座標を取得する。
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetPosition()
    {
        Vector2Int pos = new Vector2Int();
        if (mover != null)
        {
            mover.GetCurrentPosition(ref pos);
        }
        return pos;
    }

    /// <summary>
    /// アニメータの更新処理を行う。
    /// </summary>
    private void UpdateAnimator()
    {
        float speed = 0.0f;
        if (mover != null)
        {
            speed = mover.GetSpeed();
        }

        if (animator != null)
        {
            animator.SetFloat(SpeedParamName, speed);
        }
    }

    /// <summary>
    /// コライダーと重なった際に呼ばれるコールバック。
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // 相手がアイテムの場合
        if (other.tag == TagItems)
        {
            if (TileManager.Instance.IsInEasing())
            {
                return; // タイルがアニメ中の場合は何もしない
            }

            // アイテムコントローラを取得
            ItemController itemController = other.gameObject.GetComponent<ItemController>();
            if (itemController != null)
            {
                // アイテムのタイプによってスコアを加算する
                Core.SpawnType type = itemController.GetSpawnType();
                AddScore(type);
                // パーティクルアニメを再生
                ParticleManager.Instance.PlayParticle(type, other.gameObject);
                // アイテム獲得SEを再生
                SeManager.Instance.PlaySe(SeType.seGetScore);
                // キャラクターのアイテム獲得時の処理を行う
                float delay = SeManager.Instance.GetDuration(SeType.seGetScore);
                CharacterManager.Instance.ScoreGet(delay);
                // Debug.LogFormat("CharactorControll.OnTriggerEnter type={0}", type);
            }

            // 相手のオブジェクトを破棄する
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// スコアを加算する
    /// </summary>
    /// <param name="type">アイテムのスポーンタイプ</param>
    private void AddScore(Core.SpawnType type)
    {
        // int currentScore = PlayerManager.Instance.GetScore();
        // int currentRateLevel = PlayerManager.Instance.GetRateLevel();
        // float currentRate = PlayerManager.Instance.GetClampRate();
        // Debug.LogFormat("BEFORE score:{0} level:{1} rate:{2} ", currentScore, currentRateLevel, currentRate);

        // アイテムの加算スコアを取得
        int score = SpawnManager.Instance.GetScore(type);
        // アイテムのスコアレート加算率を取得
        float rate = SpawnManager.Instance.GetScoreRate(type);

        if (score > 0)
        {
            // スコアレートを加算する
            PlayerManager.Instance.AddRate(rate);
            // 現在のスコアレベルを取得する
            int level = PlayerManager.Instance.GetRateLevel();
            // スコアレベルに応じたレートを取得する
            // float だと情報落ちが発生するので100倍した整数値を用いる
            int scoreRateX100 = ScoreManager.Instance.GetScoreRateX100(level);
            // スコアを加算する
            // レートは100倍の値なので100分の1にする。
            int addScore = score * scoreRateX100 / 100;
            PlayerManager.Instance.AddScore(addScore);
            // Debug.LogFormat("ADD scoreBase:{0} rate:{1} addScore:{2} ", score, scoreRate, addScore);
        }

        // currentScore = PlayerManager.Instance.GetScore();
        // currentRateLevel = PlayerManager.Instance.GetRateLevel();
        // currentRate = PlayerManager.Instance.GetClampRate();
        // Debug.LogFormat("AFTER score:{0} level:{1} rate:{2} ", currentScore, currentRateLevel, currentRate);
    }


}
