using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテム制御クラス
/// </summary>
public class ItemController : MonoBehaviour
{
    /// <summary>
    /// アイテムのタイプ
    /// </summary>
    private Core.SpawnType spawnType;

    /// <summary>
    /// アイテムのタイプを設定する。
    /// </summary>
    /// <param name="type">アイテムタイプ</param>
    public void SetSpawnType(Core.SpawnType type)
    {
        spawnType = type;
    }

    /// <summary>
    /// アイテムのタイプを取得する。
    /// </summary>
    /// <returns>アイテムタイプ</returns>
    public Core.SpawnType GetSpawnType()
    {
        return spawnType;
    }
}
