using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Voice 管理クラス
/// </summary>
public class VoiceManager : SoundManager
{
    public static VoiceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CraeteAudioMap();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 指定したボイスを再生する。
    /// </summary>
    /// <param name="type">再生するボイスのタイプ</param>
    public void PlayVoice(VoiceType type)
    {
        // Debug.Log(string.Format("PlayVoice() called. type={0}", type));

        string strType = type.ToString();
        PlayVoice(strType);
    }

    /// <summary>
    /// 指定したボイスを再生する
    /// </summary>
    /// <param name="strType">再生するボイスのタイプ名</param>
    public void PlayVoice(string strType)
    {
        // Debug.Log(string.Format("PlayVoice() called. strType={0}", strType));
        if (audioMap.ContainsKey(strType))
        {
            AudioClip clip = audioMap[strType];
            if (clip != null)
            {
                PlayOneShot(clip);
            }
            else
            {
                Debug.Log(string.Format("PlayVoice() '{0}' audioClip is null.", strType));
            }
        }
        else
        {
            Debug.Log(string.Format("PlayVoice() '{0}' not found.", strType));
        }
    }

    /// <summary>
    /// 指定秒数待機したあと、指定したボイスを再生する。
    /// </summary>
    /// <param name="type">再生するボイスのタイプ</param>
    /// <param name="delay">待機する秒数</param>
    /// <returns>起動したCoroutine</returns>
    public Coroutine PlayVoice(VoiceType type, float delay)
    {
        // Debug.Log(string.Format("PlayVoice() called. type={0} delay={1}", type, delay));

        string strType = type.ToString();
        return PlayVoice(strType, delay);
    }

    /// <summary>
    /// 指定秒数待機したあと、指定したボイスを再生する。
    /// </summary>
    /// <param name="strType">再生するボイスのタイプ名</param>
    /// <param name="delay">待機する秒数</param>
    /// <returns>起動したCoroutine</returns>
    public Coroutine PlayVoice(string strType, float delay)
    {
        // Debug.Log(string.Format("PlayVoice() called. strType={0} delay={1}", strType, delay));
        if (audioMap.ContainsKey(strType))
        {
            AudioClip clip = audioMap[strType];
            if (clip != null)
            {
                return PlayOneShot(clip, delay);
            }
            else
            {
                Debug.Log(string.Format("PlayVoice() '{0}' audioClip is null.", strType));
                return null;
            }
        }
        else
        {
            Debug.Log(string.Format("PlayVoice() '{0}' not found.", strType));
            return null;
        }
    }
    public float GetDuration(VoiceType type)
    {
        string strType = type.ToString();
        return GetDuration(strType);
    }
}
