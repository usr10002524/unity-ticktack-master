using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ管理クラス
/// </summary>
public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// カメラのタイプ
    /// </summary>
    public enum CameraType
    {
        // ステージ開始演出カメラ
        OpeningCamera,
        // ゲーム中カメラ
        MainCamera,
    }

    /// <summary>
    /// カメラ情報
    /// </summary>
    [System.Serializable]
    public class CameraInfo
    {
        /// <summary>
        /// カメラタイプ
        /// </summary>
        public CameraType cameraType;
        /// <summary>
        /// カメラオブジェクト
        /// </summary>
        public GameObject cameraObjct;
    }

    /// <summary>
    /// カメラ情報
    /// </summary>
    [SerializeField] private List<CameraInfo> cameraInfos;
    /// <summary>
    /// 現在のカメラ
    /// </summary>
    [SerializeField] private CameraType currentCamera;
    /// <summary>
    /// 初期カメラ
    /// </summary>
    /// <value></value>
    [SerializeField] private CameraType initCamera;

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    public static CameraManager Instance { get; private set; }

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
        SetCamera(initCamera);
    }

    /// <summary>
    /// カメラをセットする。
    /// </summary>
    /// <param name="type">セットするカメラのタイプ</param>
    public void SetCamera(CameraType type)
    {
        AllOff();
        GameObject cameraObject = FindCamera(type);
        if (cameraObject != null)
        {
            cameraObject.SetActive(true);
        }
    }

    /// <summary>
    /// カメラリストから、指定したタイプのカメラを検索する。
    /// </summary>
    /// <param name="type">検索するカメラのタイプ</param>
    /// <returns>指定タイプのカメラが有る場合はゲームオブジェクトを、そうでない場合はnullを返す。</returns>
    private GameObject FindCamera(CameraType type)
    {
        CameraInfo cameraInfo = cameraInfos.Find(
            delegate (CameraInfo info)
            {
                return (type == info.cameraType);
            }
        );

        if (cameraInfo == null)
        {
            return null;
        }
        else
        {
            return cameraInfo.cameraObjct;
        }
    }

    /// <summary>
    /// すべてのカメラを非アクティブにする
    /// </summary>
    private void AllOff()
    {
        foreach (var info in cameraInfos)
        {
            info.cameraObjct.SetActive(false);
        }
    }
}
