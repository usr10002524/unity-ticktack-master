using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// WebGLでローカルストレージを呼び出すクラス
/// 内部で、ローカルストレージ用プラグインとやり取りを行っている
/// AtsumaruAPIが使えないときの代替手段
/// </summary>
public class LocalStorageAPI : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    
    [DllImport("__Internal")]
    private static extern void loadLocalData(string gameObject, string methodName);

    [DllImport("__Internal")]
    private static extern void saveLocalData(string gameObject, string methodName, string dataJson);

    [DllImport("__Internal")]
    private static extern void deleteLocalData(string gameObject, string methodName, string dataJson);
#else
    //エディタ用のダミー関数
    private static void loadLocalData(string gameObject, string methodName) { }
    private static void saveLocalData(string gameObject, string methodName, string dataJson) { }
    private static void deleteLocalData(string gameObject, string methodName, string dataJson) { }
#endif
    private bool localDataLoaded;
    private string localDataJson;

    public static LocalStorageAPI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// サーバデータのロードを行う
    /// </summary>
    /// <returns>ロード開始したかどうか</returns>
    public bool LoadLocalData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        localDataLoaded = false;

        loadLocalData("LocalStorage", "ReceiveLoadLocalData");
        // Debug.Log(string.Format("LoadServerData() called."));
        return true;
#else
        // ロード済みとする
        localDataLoaded = true;
        localDataJson = "";
        // Debug.Log(string.Format("LoadServerData() called.(dummy loaded)"));
        return true;
#endif
    }

    /// <summary>
    /// プラグインからのサーバデータロードの結果を受けるコールバック
    /// </summary>
    public void ReceiveLoadLocalData(string json)
    {
        // Debug.Log(string.Format("ReceiveLoadLocalData() called."));
        // Debug.Log(string.Format("ReceiveLoadLocalData() json={0}.", json));

        ReceiveStat data = JsonUtility.FromJson<ReceiveStat>(json);

        if (data.stat != AtsumaruAPI.SUCCESS)
        {
            return; // ロード失敗
        }

        localDataJson = json;
        localDataLoaded = true;
    }

    /// <summary>
    /// ローカルストレージのロードが完了したか確認する
    /// </summary>
    /// <returns>取得中はfalse、完了した場合はtrueを返す。</returns>
    public bool IsLocalDataLoaded()
    {
        return localDataLoaded;
    }

    /// <summary>
    /// ローカルストレージのデータを取得する
    /// </summary>
    /// <returns>ローカルストレージのデータ</returns>
    public string GetLocalDataJson()
    {
        return localDataJson;
    }

    /// <summary>
    /// ローカルストレージに保存する
    /// </summary>
    /// <param name="json">保存するJsonデータ</param>
    /// <returns>結果可否は今のところチェックしていません。常にtrueを返します。</returns>
    public bool SaveLocalData(string json)
    {
        // Debug.Log(string.Format("SaveLocalData() called."));
        // Debug.Log(string.Format("SaveLocalData() json={0}.", json));

        localDataJson = json;
        LocalStorageAPI.saveLocalData("LocalStorage", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// ローカルストレースからデータを削除する
    /// </summary>
    /// <param name="json">削除するJsonデータ</param>
    /// <returns>結果可否は今のところチェックしていません。常にtrueを返します。</returns>
    public bool DeleteLocalData(string json)
    {
        LocalStorageAPI.deleteLocalData("LocalStorage", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// プラグインからの共通の汎用返答を受け取るクラス
    /// </summary>
    [System.Serializable]
    private class ReceiveStat
    {
        public int stat;
    }

    /// <summary>
    /// プラグインからの汎用返答を受けるコールバック
    /// </summary>
    /// <param name="json"></param>
    public void ReceiveCommonStat(string json)
    {
        // Debug.Log(string.Format("ReceiveCommonStat() called."));
        // Debug.Log(string.Format("ReceiveCommonStat() json={0}.", json));

        ReceiveStat data = JsonUtility.FromJson<ReceiveStat>(json);
        // Debug.Log(string.Format("ReceiveCommonStat() load stat={0}.", data.stat));
    }
}
