using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サウンドボリュームコントローラ
/// </summary>
public class SoundVolumeController : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private GameObject sliderObject;


    private SoundPanelController panelController;
    private SoundSliderController sliderController;

    private bool toggleButton;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        if (buttonObject != null)
        {
            panelController = buttonObject.GetComponent<SoundPanelController>();
        }
        if (sliderObject != null)
        {
            sliderController = sliderObject.GetComponent<SoundSliderController>();
            sliderObject.SetActive(false);
        }

        toggleButton = false;
        LoadSetitng();
    }

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        LoadSetitng();
    }

    /// <summary>
    /// サウンドボリュームのスライダーを表示する
    /// </summary>
    private void OpenSlider()
    {
        if (sliderObject != null)
        {
            sliderObject.SetActive(true);
        }
    }

    /// <summary>
    /// サウンドボリュームのスライダーを非表示にする
    /// </summary>
    private void CloseSlider()
    {
        if (sliderObject != null)
        {
            sliderObject.SetActive(false);
        }
        SaveSetting();
    }

    /// <summary>
    /// 設定を取得する
    /// </summary>
    private void LoadSetitng()
    {
        if (AtsumaruAPI.Instance.IsValid())
        {

        }
        else
        {
            string json = LocalStorageAPI.Instance.GetLocalDataJson();
            AtsumaruAPI.ServerDataItems serverDataItems = JsonUtility.FromJson<AtsumaruAPI.ServerDataItems>(json);
            if (serverDataItems != null && serverDataItems.data != null)
            {
                foreach (var item in serverDataItems.data)
                {
                    if (item.key == SoundVolumeData.key)
                    {
                        Debug.Log(string.Format("SoundVolumeController.LoadSetitng {0}", item.value));

                        SoundVolumeData soundVolume = JsonUtility.FromJson<SoundVolumeData>(item.value);
                        OnChangeValue(soundVolume.masterVolume);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 設定を保存する
    /// </summary>
    private void SaveSetting()
    {
        if (AtsumaruAPI.Instance.IsValid())
        {

        }
        else
        {
            // 保存データを作成
            SoundVolumeData data = new SoundVolumeData();
            data.dataVersion = SoundVolumeData.version;
            data.masterVolume = AudioListener.volume;

            AtsumaruAPI.ServerDataItems serverDataItems = new AtsumaruAPI.ServerDataItems();
            serverDataItems.data = new AtsumaruAPI.DataItem[1];

            // 保存データを json 形式にする
            AtsumaruAPI.DataItem item = new AtsumaruAPI.DataItem();
            item.key = SoundVolumeData.key;
            item.value = JsonUtility.ToJson(data);
            serverDataItems.data[0] = item;

            string json = JsonUtility.ToJson(serverDataItems);
            Debug.Log(string.Format("SoundVolumeController.SaveSetting {0}", json));
            LocalStorageAPI.Instance.SaveLocalData(json);
        }
    }

    /// <summary>
    /// スライダーの開閉をトグルする
    /// </summary>
    public void OnTggleButton()
    {
        if (toggleButton)
        {
            CloseSlider();
        }
        else
        {
            OpenSlider();
        }
        toggleButton = !toggleButton;
    }

    /// <summary>
    /// スライダーを閉じる
    /// </summary>
    public void OnClose()
    {
        if (toggleButton)
        {
            CloseSlider();
            toggleButton = !toggleButton;
        }
    }

    /// <summary>
    /// スライダーの値が変更されたときに呼ばれる
    /// </summary>
    /// <param name="value">変更された値</param>
    public void OnChangeValue(float value)
    {
        if (sliderController != null)
        {
            sliderController.SetVolume(value);
        }
        if (panelController != null)
        {
            panelController.SetMute(!(value > 0));
        }
        AudioListener.volume = value;
    }

}
