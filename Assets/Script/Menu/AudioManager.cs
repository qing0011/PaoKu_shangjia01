using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioManager Instance {  get; private set; }

    public static AudioClip clickSound;
    private static AudioSource audioSource;
    //设置里的音乐和音效
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    void Awake()
    {
        //单例模式
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//跨场景保留
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            return;
        }
        //初始化主音效源
        audioSource =sfxSource;

        // 确保 Resources/Sounds/click.wav 存在
        clickSound = Resources.Load<AudioClip>("Sounds/BonusBackground"); 

        if(clickSound != null)
        {
            Debug.Log("未找到Click 音效，路径：Resources/Sounds/音效");
        }
    }
    //播放点击音效
    public static void PlayClick()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogWarning("音效播放失败：audioSource 或 clickSound 为 null");
        }
    }
    public static void PlaySound(AudioClip clip)
    {
        if( clip !=null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("播放音效失败：clip 或 audioSource 为null");
        }
    }
    //播放任意音效（一次性）
    public void SetMusicEnabled(bool enabled)
    {
        bgmSource.mute = !enabled;
    }
    //播放任意音效（一次性）
    public void SetSoundEnabled(bool enabled)
    {
        sfxSource.mute = !enabled;
    }
   
}
