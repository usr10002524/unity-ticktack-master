using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// パスの描画を行うクラス
/// </summary>
public class PathRenderer : MonoBehaviour
{
    /// <summary>
    /// パスを持つGameObject
    /// </summary>
    [SerializeField]
    private GameObject pathObject;

    /// <summary>
    /// 区間の分割数
    /// </summary>
    [SerializeField]
    [Range(1, 8)]
    private int tessellation;

    /// <summary>
    /// Y方向のオフセット
    /// </summary>
    [SerializeField]
    private float yOffset;

    /// <summary>
    /// パス情報
    /// </summary>
    private CinemachinePathBase cinemachinePath;
    /// <summary>
    /// ラインレンダラ
    /// </summary>
    private LineRenderer lineRenderer;

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        UpdatePath();
    }

    /// <summary>
    /// パスの描画更新を行う
    /// </summary>
    private void UpdatePath()
    {
        SetupCompornent();
        RenderPath();
    }

    /// <summary>
    /// コンポーネントの確認と準備を行う
    /// </summary>
    private void SetupCompornent()
    {
        if (cinemachinePath == null)
        {
            if (pathObject != null)
            {
                cinemachinePath = pathObject.GetComponent<CinemachinePathBase>();
            }
        }
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    /// <summary>
    /// パスの描画を行う
    /// </summary>
    private void RenderPath()
    {
        if (cinemachinePath == null)
        {
            return;
        }
        if (lineRenderer == null)
        {
            return;
        }

        tessellation = Mathf.Max(tessellation, 1);
        float step = 1.0f / tessellation;
        int positionCount = (int)(cinemachinePath.MaxPos - cinemachinePath.MinPos) * tessellation + 1;

        lineRenderer.positionCount = positionCount;
        for (int i = 0; i < positionCount; i++)
        {
            Vector3 pos = cinemachinePath.EvaluatePosition(i * step) + new Vector3(0, yOffset, 0);
            lineRenderer.SetPosition(i, pos);
        }
    }
}
