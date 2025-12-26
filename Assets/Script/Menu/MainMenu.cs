using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("按钮组件")]
    public Button playButton;
    public Button resumeButton;
    public Button signInButton;
    public Button settingButton;
   // public Button storeButton;

    [Header("UI组件")]
    public Text bestScoreText;
    [SerializeField] private GameObject settingsPanel;
    public GameObject storePanel;
    public GameObject signInPanel;
    public Text coinText;//显示金币
    [Header("音效")]
    public AudioSource startGameAudioSource;//开始游戏音效

    private void Start()
    {
        //初始化玩家数据
        int coins = PlayerPrefs.GetInt("Coins", 0);//金币
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);//最高分显示
        // 更新UI显示
        if (coinText != null)
        {
            coinText.text = "金币：" + coins;
        }
            
        if (bestScoreText != null)
        {
            bestScoreText.text = "最高得分：" + bestScore;
        }
        // 确保按钮都绑定成功
        if (playButton != null) playButton.onClick.AddListener(OnPlay);
        if (resumeButton != null) resumeButton.onClick.AddListener(OnResetHighScore);
        if (signInButton != null) signInButton.onClick.AddListener(OnSignIn);
        if (settingButton != null) settingButton.onClick.AddListener(OnOpenSettings);
       // if (storeButton != null) storeButton.onClick.AddListener(OnOpenStore);

       

        UpdateHighScoreDisplay();
    }

    void OnPlay()
    {
        AudioManager.PlayClick();
        //播放开始音效
        if(startGameAudioSource != null)
        {
           startGameAudioSource.Play();
        }
        SceneManager.LoadScene("Scene_01");
    }

    void OnResetHighScore()
    {
        AudioManager.PlayClick();
        PlayerPrefs.SetInt("BestScore", 0);
        PlayerPrefs.Save(); // 
        UpdateHighScoreDisplay();
    }
 
    void OnSignIn()
    {
        AudioManager.PlayClick();

        // 显示签到面板
        signInPanel.SetActive(true);

        // 更新签到UI状态
        if (signInPanel.TryGetComponent<DailyRewardUI>(out var dailyRewardUI))
        {
            dailyRewardUI.UpdateUI();
        }

        // 尝试立即签到(可选，根据你的需求决定是否自动签到)
        // DailyRewardManager.Instance.SignToday();
    }

    void OnOpenSettings()
    {
        AudioManager.PlayClick();
        settingsPanel.SetActive(true);
    }

    void OnOpenStore()
    {
        AudioManager.PlayClick();
        storePanel.SetActive(true);
        StoreManager.UpdateUI();
    }

    void UpdateHighScoreDisplay()
    {
        int score = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = "最高积分：" + score;
    }
    
}
