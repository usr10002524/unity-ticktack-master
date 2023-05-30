using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パーティクル管理クラス
/// </summary>
public class ParticleManager : MonoBehaviour
{
    /// <summary>
    /// パーティクル情報
    /// </summary>
    [System.Serializable]
    public class ParticleObject
    {
        /// <summary>
        /// スポーンの種別
        /// </summary>
        public Core.SpawnType type;

        /// <summary>
        /// スポーンするオブジェクト
        /// </summary>
        public GameObject particlePrefab;
    }

    /// <summary>
    /// スポーンオブジェクトのリスト
    /// </summary>
    [SerializeField] private List<ParticleObject> particleObjects;
    /// <summary>
    /// パーティクル表示の親オブジェクト
    /// </summary>
    /// <value></value>
    [SerializeField] private GameObject particleBase;

    /// <summary>
    /// シングルトンのインスタンス
    /// </summary>
    public static ParticleManager Instance { get; private set; }


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
    /// パーティクルを再生する。
    /// </summary>
    /// <param name="type">パーティクルのタイプ</param>
    /// <param name="obj">パーティクルを生成する際の親オブジェクト</param>
    public void PlayParticle(Core.SpawnType type, GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        GameObject particlePrefab = FindPrefab(type);
        if (particlePrefab == null)
        {
            return;
        }

        GameObject particleObject = Instantiate(particlePrefab, particleBase.transform);
        particleObject.transform.position = obj.transform.position;
    }

    /// <summary>
    /// 指定したアイテムタイプに対応するパーティクルプレファブを検索する。
    /// </summary>
    /// <param name="type">アイテムタイプ</param>
    /// <returns>パーティクルのプレファブ</returns>
    private GameObject FindPrefab(Core.SpawnType type)
    {
        ParticleObject particlePrefab = particleObjects.Find(
            delegate (ParticleObject particleObj)
            {
                return (type == particleObj.type);
            }
        );

        if (particlePrefab == null)
        {
            return null;
        }
        else
        {
            return particlePrefab.particlePrefab;
        }
    }
}
