using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽選クラス
/// </summary>
public class Lottery
{
    /// <summary>
    /// 重み付きリストの抽選を行う
    /// </summary>
    /// <param name="weights">重みリスト</param>
    /// <returns>抽選結果。重みリストのインデックスを返す</returns>
    public static int LotteryWeight(List<int> weights)
    {
        int totalProb = 0;
        foreach (var val in weights)
        {
            totalProb += val;
        }

        if (totalProb == 0)
        {
            throw new System.ArgumentException("totalProb is zero.");
        }

        int lottery = Random.Range(0, totalProb);
        for (int i = 0; i < weights.Count; i++)
        {
            if (lottery < weights[i])
            {
                return i;
            }

            lottery -= weights[i];
            if (lottery < 0)
            {
                throw new System.ArgumentException("lottery is negative value.");
            }
        }

        // ここには来ないはず
        throw new System.ArgumentException("lottery index not found.");
    }
}
