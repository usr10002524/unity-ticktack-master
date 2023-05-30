using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasingController : MonoBehaviour
{
    /// <summary>
    /// 制御するゲームオブジェクト
    /// </summary>
    [SerializeField] private GameObject easeObject;

    /// <summary>
    /// オブジェクトに設定されているイージングのリスト
    /// </summary>
    private List<Easing> easeList;

    /// <summary>
    /// イージングを開始したかどうか
    /// </summary>
    private bool isStarted;


    /// <summary>
    /// 初期化時の処理を行う。
    /// </summary>
    public virtual void OnInit()
    {
        if (easeList == null)
        {
            easeList = new List<Easing>();
        }
        easeList.Clear();

        Easing[] easings = GetComponents<Easing>();
        foreach (var item in easings)
        {
            item.SetGameObject(easeObject);
            item.OnInit();
            easeList.Add(item);
        }

        isStarted = false;
    }

    /// <summary>
    /// 初期化時の処理を行う。
    /// </summary>
    /// <param name="obj">制御する対象のGameObject</param>
    public virtual void OnInit(GameObject obj)
    {
        easeObject = obj;
        OnInit();
    }

    /// <summary>
    /// カットインを開始する。
    /// </summary>
    public virtual void StartEasing()
    {
        foreach (var item in easeList)
        {
            item.StartEasing();
        }
        isStarted = true;
    }

    /// <summary>
    /// カットインを開始したかどうかチェックする
    /// </summary>
    /// <returns>開始した場合はtrue、そうでない場合はfalseを返す。</returns>
    public virtual bool IsStarted()
    {
        return isStarted;
    }

    /// <summary>
    /// カットイン中かどうかチェックする。
    /// </summary>
    /// <returns>カットイン中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public virtual bool IsInEasing()
    {
        foreach (var item in easeList)
        {
            if (item.IsInEasing())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// カットインが終了したかどうかチェックする。
    /// </summary>
    /// <returns>カットインが終了した場合はtrue、そうでない場合はfalseを返す。</returns>
    public virtual bool IsFinished()
    {
        foreach (var item in easeList)
        {
            if (!item.IsFinished())
            {
                return false;
            }
        }

        return true;
    }
}
