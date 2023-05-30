using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム開始時のカメラ演出切り替えクラス
/// </summary>
public class OpeningCameraController : MonoBehaviour
{
    /// <summary>
    /// ゲーム開始時のカメラアニメ終了時に呼ばれる。
    /// ここでゲーム中のカメラに切り替える。
    /// </summary>
    public void EndAnimation()
    {
        // Debug.LogFormat("OpeningCameraController.EndAnimation() called.");
        CameraManager.Instance.SetCamera(CameraManager.CameraType.MainCamera);
    }
}
