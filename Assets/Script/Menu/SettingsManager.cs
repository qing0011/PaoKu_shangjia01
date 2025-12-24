
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("组件引用")]
    //public Slider volumeSlider;
    public Button closeButton;
   // public Button saveButton;
    [Header("面板对象")]
    public GameObject settingsPanel; // 你要关闭的设置弹窗对象
    [Header("按钮")]
    
    public Toggle soundToggle;
    public Toggle musicToggle;
    public Slider sensitivitySlider;



    void Start()
    {
     
       

        closeButton.onClick.AddListener(CloseSettings);
       // saveButton.onClick.AddListener(() => gameObject.SetActive(true));

        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        // 初始化 UI 显示
        soundToggle.isOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        musicToggle.isOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
    }

    private void CloseSettings()
    {

        SaveSettings(); // 关闭前保存设置
        settingsPanel.SetActive(false); //  隐藏设置面板
        AudioManager.PlayClick(); // 播放关闭音效
    }
    //音效
    private void OnSoundToggleChanged(bool ison)
    {
        PlayerPrefs.SetInt("SoundOn",ison ?  1 : 0);
        Debug.Log("音效："+(ison ? "开启" :"关闭"));

    }
    //背景音乐
    private void OnMusicToggleChanged(bool ison)
    {
        PlayerPrefs.SetInt("MusicOn", ison ? 1 : 0);
        Debug.Log("音乐：" + (ison ? "开启" : "关闭"));

    }
    //灵敏度滑杆
    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        Debug.Log("灵敏度：" + value.ToString("F2"));
    }


    void SaveSettings()
    {
       // PlayerPrefs.SetInt("SoundEnabled", soundToggle.isOn ? 1 : 0);
       // PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        Debug.Log("设置已保存");
    }
    
}
