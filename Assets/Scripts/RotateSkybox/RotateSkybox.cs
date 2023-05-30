using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スカイボックスを回転させるクラス
/// </summary>
public class RotateSkybox : MonoBehaviour
{
    /// <summary>
    /// 回転速度
    /// </summary>    
    [SerializeField] private float rotateSpeed = 0.5f;

    /// <summary>
    /// スカイボックスのマテリアル
    /// </summary>
    private Material skyboxMaterial;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        skyboxMaterial = RenderSettings.skybox;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        float rotation = skyboxMaterial.GetFloat("_Rotation");
        rotation = Mathf.Repeat(rotation + rotateSpeed * Time.deltaTime, 360.0f);
        skyboxMaterial.SetFloat("_Rotation", rotation);
    }

}
