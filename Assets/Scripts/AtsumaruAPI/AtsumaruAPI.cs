using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


/// <summary>
/// アツマールAPIを呼び出すクラス
/// 内部で、アツマール用プラグインとやり取りを行っている
/// </summary>
public class AtsumaruAPI : MonoBehaviour
{
    /// <summary>
    /// サーバデータの型
    /// </summary>
    [System.Serializable]
    public class DataItem
    {
        public string key;
        public string value;
    }

    /// <summary>
    /// サーバデータのロード結果を受け取るクラス
    /// </summary>
    [System.Serializable]
    public class ServerDataItems
    {
        public DataItem[] data;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool atsumaru_isValid();

    [DllImport("__Internal")]
    private static extern float atsumaru_getVolume();

    [DllImport("__Internal")]
    private static extern void atsumaru_onChangeVolume(string gameObject, string methodName);
    
    [DllImport("__Internal")]
    private static extern void atsumaru_onScreenCapture(string gameObject, string methodName);

    [DllImport("__Internal")]
    private static extern void atsumaru_setScreenCapture(byte[] img, int size);
    
    [DllImport("__Internal")]
    private static extern void atsumaru_loadServerData(string gameObject, string methodName);

    [DllImport("__Internal")]
    private static extern void atsumaru_saveServerData(string gameObject, string methodName, string dataJson);

    [DllImport("__Internal")]
    private static extern void atsumaru_deleteServerData(string gameObject, string methodName, string dataJson);

    [DllImport("__Internal")]
    private static extern void atsumaru_saveScoreBoard(string gameObject, string methodName, int boardId, int score);

    [DllImport("__Internal")]
    private static extern void atsumaru_displayScoreBoard(string gameObject, string methodName, int boardId);
#else
    //エディタ用のダミー関数
    private static bool atsumaru_isValid() { return false; }
    private static float atsumaru_getVolume() { return 1.0f; }
    private static void atsumaru_onChangeVolume(string gameObject, string methodName) { }
    private static void atsumaru_onScreenCapture(string gameObject, string methodName) { }
    private static void atsumaru_setScreenCapture(byte[] img, int size) { }
    private static void atsumaru_loadServerData(string gameObject, string methodName) { }
    private static void atsumaru_saveServerData(string gameObject, string methodName, string dataJson) { }
    private static void atsumaru_deleteServerData(string gameObject, string methodName, string dataJson) { }
    private static void atsumaru_saveScoreBoard(string gameObject, string methodName, int boardId, int score) { }
    private static void atsumaru_displayScoreBoard(string gameObject, string methodName, int boardId) { }

#endif
    public static readonly int SUCCESS = 0;
    public static readonly int FAIL = 1;

    private bool apiIsValid;
    private bool serverDataLoaded;
    private string serverDataJson;
    public static AtsumaruAPI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // アツマールAPIの使用可否を確認
        apiIsValid = atsumaru_isValid();
        Debug.Log(string.Format("AtsumaruAPI : {0}", apiIsValid));
    }


    // Start is called before the first frame update
    private void Start()
    {
        if (IsValid())
        {
            float volume = atsumaru_getVolume();
            SetMasterVolume(volume);

            // サウンドボリューム変更コールバックを設定
            atsumaru_onChangeVolume("Atsumaru", "ChangeVolume");
            // 画面キャプチャコールバックを設定
            atsumaru_onScreenCapture("Atsumaru", "CaptuerScreen");
        }
    }


    /// <summary>
    /// アツマールAPIが使用可能かどうか確認する。
    /// </summary>
    /// <returns>trueの場合は使用可能。falseの場合は使用不可。</returns>
    public bool IsValid()
    {
        return apiIsValid;
    }

    /// <summary>
    /// ボリューム情報
    /// プラグインからはJSONで渡されるので、こちらのクラスで変換する
    /// </summary>
    [System.Serializable]
    private class JsonVolume
    {
        public float volume;
    }

    /// <summary>
    /// プラグインからのサウンドボリューム変更を受けるコールバック
    /// </summary>
    /// <param name="json">サウンド情報が格納されたJSON</param>
    public void ChangeVolume(string json)
    {
        JsonVolume jsonVolume = JsonUtility.FromJson<JsonVolume>(json);
        // Debug.Log(string.Format("ChangeVolume() json:{0}", jsonVolume.volume));

        SetMasterVolume(jsonVolume.volume);
    }


    /// <summary>
    /// マスターボリュームの設定
    /// </summary>
    /// <param name="volume">設定するボリューム(0.0-1.0)</param>
    private void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }


    /// <summary>
    /// プラグインからの画面キャプチャ要求を受けるコールバック
    /// </summary>
    public void CaptuerScreen()
    {
        // Debug.Log(string.Format("CaptuerScreen() called."));
        StartCoroutine(CaptureCoroutine());
    }

    /// <summary>
    /// 画面キャプチャの本処理。
    /// キャプチャを取得後、データをプラグイン側に渡す。
    /// </summary>
    /// <returns></returns>
    private IEnumerator CaptureCoroutine()
    {
        // Debug.Log(string.Format("CaptureCoroutine() called."));
        //描画処理が終わるのを待つ
        yield return new WaitForEndOfFrame();

        // 画面キャプチャを取得
        Texture2D tex = new Texture2D(Screen.width, Screen.height);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();
        byte[] img = tex.EncodeToPNG();

        //プラグイング側にキャプチャデータを渡す
        atsumaru_setScreenCapture(img, img.Length);
        // Debug.Log(string.Format("CaptureCoroutine() leave."));
    }

    /// <summary>
    /// サーバデータのロードを行う
    /// </summary>
    /// <returns>ロード開始したかどうか</returns>
    public bool LoadServerData()
    {
        serverDataLoaded = false;
        if (!IsValid())
        {
            // Debug.Log(string.Format("LoadServerData() AtsmaruApi not in work."));
            serverDataLoaded = true;
            return false;
        }

        atsumaru_loadServerData("Atsumaru", "ReceiveLoadServerData");
        // Debug.Log(string.Format("LoadServerData() called."));
        return true;
    }

    /// <summary>
    /// プラグインからのサーバデータロードの結果を受けるコールバック
    /// </summary>
    public void ReceiveLoadServerData(string json)
    {
        // Debug.Log(string.Format("ReceiveLoadServerData() called."));
        // Debug.Log(string.Format("ReceiveLoadServerData() json={0}.", json));

        ReceiveStat data = JsonUtility.FromJson<ReceiveStat>(json);
        if (data.stat != SUCCESS)
        {
            return; // ロード失敗
        }

        serverDataJson = json;
        serverDataLoaded = true;
    }

    /// <summary>
    /// サーバデータのロードが完了したか確認する
    /// </summary>
    /// <returns>取得中はfalse、完了した場合はtrueを返す。
    /// API使用不可の場合はロード待ちでハマることを防ぐため、trueを返す。</returns>
    public bool IsServerDataLoaded()
    {
        return serverDataLoaded;
    }

    /// <summary>
    /// サーバデータを取得する
    /// </summary>
    /// <returns>ロードしたサーバデータを返す。</returns>
    public string GetServerDataJson()
    {
        return serverDataJson;
    }

    /// <summary>
    /// サーバデータを保存する
    /// </summary>
    /// <param name="json">保存するJsonデータ</param>
    /// <returns>保存開始した場合はtrue, API使用不可の場合はfalseを返す。</returns>
    public bool SaveServerData(string json)
    {
        if (!IsValid())
        {
            // Debug.Log(string.Format("SaveServerData() AtsmaruApi not in work."));
            return false;
        }

        atsumaru_saveServerData("Atsumaru", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// サーバデータを削除する
    /// </summary>
    /// <param name="json">削除するJsonデータ</param>
    /// <returns>削除開始した場合はtrue, API使用不可の場合はfalseを返す。</returns>
    public bool DeleteServerData(string json)
    {
        if (!IsValid())
        {
            // Debug.Log(string.Format("DeleteServerData() AtsmaruApi not in work."));
            return false;
        }

        atsumaru_deleteServerData("Atsumaru", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// スコアボードにスコアを保存する
    /// </summary>
    /// <param name="boardId">スコアボードのID(1-10)</param>
    /// <param name="score">スコア</param>
    /// <returns></returns>
    public bool SaveScore(int boardId, int score)
    {
        if (!IsValid())
        {
            // Debug.Log(string.Format("SaveScore() AtsmaruApi not in work."));
            return false;
        }

        atsumaru_saveScoreBoard("Atsumaru", "ReceiveCommonStat", boardId, score);
        return true;
    }

    /// <summary>
    /// スコアボードを表示する。
    /// </summary>
    /// <param name="boardId">スコアボードのID(1-10)</param>
    /// <returns></returns>
    public bool DisplayScoreBoard(int boardId)
    {
        if (!IsValid())
        {
            // Debug.Log(string.Format("DisplayScoreBoard() AtsmaruApi not in work."));
            return false;
        }

        atsumaru_displayScoreBoard("Atsumaru", "ReceiveCommonStat", boardId);
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
        // Debug.Log(string.Format("LoadServerDataResult() load stat={0}.", data.stat));
    }
}
