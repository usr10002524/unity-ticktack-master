using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サウンド管理クラス
/// </summary>
public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// パス情報
    /// </summary>
    [System.Serializable]
    public class AudioInfo
    {
        /// <summary>
        /// オーディ名
        /// </summary>
        public string audioName;
        /// <summary>
        /// オーディオクリップ
        /// </summary>
        public AudioClip audioClip;
    }


    [SerializeField] protected List<AudioInfo> audioInfos;
    [SerializeField] private float originalVolume;

    protected AudioSource audioSource;
    protected Dictionary<string, AudioClip> audioMap;

    private Coroutine fadeCoroutine;


    protected float GetDuration(string audioName)
    {
        if (audioMap.ContainsKey(audioName))
        {
            AudioClip clip = audioMap[audioName];
            if (clip != null)
            {
                return clip.length;
            }
        }

        return 0.0f;
    }

    /// <summary>
    /// エディタで登録された、NamedAudioClipがアタッチされたゲームオブジェクトから
    /// マップデータを作成する
    /// </summary>
    protected void CraeteAudioMap()
    {
        audioMap = new Dictionary<string, AudioClip>();

        foreach (var item in audioInfos)
        {
            if (item != null)
            {
                if (!audioMap.ContainsKey(item.audioName))
                {
                    audioMap.Add(item.audioName, item.audioClip);
                }
                else
                {
                    Debug.Log(string.Format("SoundManager.CraeteAudioMap() name={0} is already entried.", item.audioName));
                }
            }
        }
    }

    /// <summary>
    /// AudioClipをPlayOneShotで再生する。
    /// </summary>
    /// <param name="clip">再生するAudioClip</param>
    protected void PlayOneShot(AudioClip clip)
    {
        if (audioSource != null)
        {
            if (clip != null)
            {
                audioSource.volume = originalVolume;
                audioSource.PlayOneShot(clip);
                // Debug.Log(string.Format("CraeteAudioMap.playAudioClip() play name={0} volume={1}", clip.name, originalVolume));
            }
            else
            {
                Debug.Log("CraeteAudioMap.playAudioClip() clip=null");
            }
        }
        else
        {
            Debug.Log("CraeteAudioMap.playAudioClip() audioSource=null");
        }
    }

    protected Coroutine PlayOneShot(AudioClip clip, float delay)
    {
        if (delay > 0.0f)
        {
            Coroutine coroutine = StartCoroutine(PlayWithDelayCoroutine(clip, delay));
            return coroutine;
        }
        else
        {
            // 即時再生
            PlayOneShot(clip);
            return null;
        }
    }

    /// <summary>
    /// AudioClipを再生する。
    /// </summary>
    /// <param name="clip">再生するAudioClip</param>
    protected void Play(AudioClip clip)
    {
        if (fadeCoroutine != null)
        {
            // フェードが動いていれば止める
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (audioSource != null)
        {
            if (clip != null)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.volume = originalVolume;
                audioSource.Play();
                // Debug.Log(string.Format("CraeteAudioMap.playAudioClip() play name={0} volume={1}", clip.name, originalVolume));

            }
            else
            {
                Debug.Log("CraeteAudioMap.playAudioClip() clip=null");
            }
        }
        else
        {
            Debug.Log("CraeteAudioMap.playAudioClip() audioSource=null");
        }
    }

    /// <summary>
    /// 再生中のAudioClipを停止する
    /// </summary>
    protected void stopAudioClip()
    {
        if (fadeCoroutine != null)
        {
            // フェードが動いていれば止める
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// 再生中のAudioClipをフェードアウト後、停止させる
    /// </summary>
    /// <param name="duration">フェードアウト時間（秒）</param>
    protected void fadeStopAudioClip(float duration)
    {
        if (fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeoutStop(duration));
        }
    }

    /// <summary>
    /// 再生中のAudioClipのボリュームを変更する
    /// </summary>
    /// <param name="duration">フェード時間</param>
    /// <param name="toVolume">フェード後の音量</param>
    protected void fadeVolumeAudioClip(float duration, float toVolume)
    {
        if (fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeVolume(duration, toVolume));
        }
    }

    /// <summary>
    /// フェードアウト＋停止用コルーチン
    /// </summary>
    /// <param name="duration">フェードアウト時間（秒）</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator FadeoutStop(float duration)
    {
        if (audioSource == null)
        {
            yield break;
        }

        bool isEnd = false;
        float fadeoutDuration = duration;
        float startVolume = audioSource.volume;
        float endVolume = 0.0f;
        float fadeTimer = 0.0f;

        if (fadeoutDuration <= 0.0f)
        {
            fadeoutDuration = 0.25f;
        }

        while (!isEnd)
        {
            yield return null;
            if (!audioSource.isPlaying)
            {
                break;  //もう止まっていれば何もしない
            }

            fadeTimer += Time.deltaTime;
            float time = Mathf.Clamp(fadeTimer / fadeoutDuration, 0.0f, 1.0f);
            float t = Mathf.Lerp(startVolume, endVolume, time);

            audioSource.volume = t;
            if (fadeTimer >= fadeoutDuration)
            {
                isEnd = true;
            }
        }

        audioSource.Stop();
        fadeCoroutine = null;
    }

    /// <summary>
    /// フェード用コルーチン
    /// </summary>
    /// <param name="duration">変化後の音量</param>
    /// <param name="toVolume">変化に要する時間（秒）</param>
    /// <returns></returns>
    private IEnumerator FadeVolume(float duration, float toVolume)
    {
        if (audioSource == null)
        {
            yield break;
        }

        bool isEnd = false;
        float fadeoutDuration = duration;
        float startVolume = audioSource.volume;
        float endVolume = toVolume;
        float fadeTimer = 0.0f;

        if (fadeoutDuration <= 0.0f)
        {
            fadeoutDuration = 0.25f;
        }

        while (!isEnd)
        {
            yield return null;
            if (!audioSource.isPlaying)
            {
                break;  //もう止まっていれば何もしない
            }

            fadeTimer += Time.deltaTime;
            float time = Mathf.Clamp(fadeTimer / fadeoutDuration, 0.0f, 1.0f);
            float t = Mathf.Lerp(startVolume, endVolume, time);

            audioSource.volume = t;
            if (fadeTimer >= fadeoutDuration)
            {
                isEnd = true;
            }
        }

        fadeCoroutine = null;
    }

    private IEnumerator PlayWithDelayCoroutine(AudioClip clip, float delay)
    {
        if (clip == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);

        PlayOneShot(clip);
    }
}
