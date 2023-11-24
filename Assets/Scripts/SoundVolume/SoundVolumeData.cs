using System;
using UnityEngine;

/// <summary>
/// サウンドボリュームデータ
/// </summary>
[System.Serializable]
public class SoundVolumeData
{
    public static readonly string key = "SoundVolume";
    public static readonly int version = 1;

    public int dataVersion; // データバージョン
    public float masterVolume;    // マスターボリューム(0-1)

    public bool IsValid()
    {
        return (dataVersion > 0);
    }
}
