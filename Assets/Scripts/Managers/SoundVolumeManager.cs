using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVolumeManager : MonoBehaviour
{
    /// <summary>
    /// サウンドボリュームプレファブのゲームオブジェクト
    /// </summary>
    [SerializeField] private GameObject volumePrefab;

    /// <summary>
    /// サウンドボリュームオブジェクトのベース
    /// </summary>
    [SerializeField] private GameObject volumeBase;

    /// <summary>
    /// サウンドボリュームオブジェクト
    /// </summary>
    private GameObject volumeObject;

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    public static SoundVolumeManager Instance { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// タイトルシーン用の初期化処理を行う
    /// </summary>
    public void Initialize()
    {
        if (volumePrefab == null)
        {
            return;
        }
        if (volumeBase == null)
        {
            return;
        }

        // ヘルプゲームオブジェクトを生成する
        volumeObject = Instantiate(volumePrefab, volumeBase.transform);
        if (volumeObject == null)
        {
            return;
        }
        ShowButton(false);
    }

    /// <summary>
    /// タイトルシーン用の初期化処理を行う
    /// </summary>
    public void InitializeTitleScene()
    {
        if (volumePrefab == null)
        {
            return;
        }
        if (volumeBase == null)
        {
            return;
        }

        // ヘルプゲームオブジェクトを生成する
        volumeObject = Instantiate(volumePrefab, volumeBase.transform);
        if (volumeObject == null)
        {
            return;
        }
        ShowButton(true);
    }

    /// <summary>
    /// ボリュームボタンの表示、非表示を行う。
    /// </summary>
    /// <param name="flag">表示するときはtrueを、非表示にするときはfalseを指定する。</param>
    public void ShowButton(bool flag)
    {
        if (volumeBase == null)
        {
            return;
        }
        volumeBase.SetActive(flag);
    }
}
