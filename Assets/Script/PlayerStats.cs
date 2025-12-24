using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

// 玩家状态统计类 - 管理分数、金币、时间等玩家数据
public class PlayerStats : MonoBehaviour
{
    // 单例模式实例
    public static PlayerStats Instance;

    [Header("UI 组件")]
    public GameObject timeText; // 时间显示文本对象
    public GameObject FailUI; // 失败界面
    public GameObject pauseUI; // 暂停界面
    public GameObject failEffect;         // 失败特效预制体（粒子系统）
    public AudioClip failSound;           // 失败音效
    [SerializeField] private TMP_Text scoreText; // 分数显示文本(序列化字段)
    [SerializeField] private TMP_Text coinText; // 金币显示文本(序列化字段)

    [Header("初始值")]
    [SerializeField] private int startSeconds = 150; // 初始倒计时时间(秒)

    // 私有变量
    private int theSeconds; // 当前剩余时间
    private static int currentScore = 0; // 静态分数变量
    private static int coins = 0; // 静态金币变量
   // 属性访问器
    public static int CurrentScore => currentScore; // 当前分数只读属性
    public static int CurrentCoins => coins; // 当前金币只读属性
    private bool isPaused = false; // 游戏是否暂停

    // 双倍得分系统
    private bool isDoubleScoreActive = false; // 是否处于双倍得分状态
    private Coroutine doubleScoreRoutine; // 双倍得分协程
    public float scoreMultiplier = 1f; // 分数乘数(默认为1)
    // Awake方法 - 初始化单例
    private void Awake()
    {
        // 单例模式实现
        if (Instance != null && Instance != this)
        {
            // 销毁重复实例
            Destroy(gameObject);
            return;
        }
       
        Instance = this;
    }
    // Start方法 - 游戏开始时调用
    private void Start()
    {
        currentScore = 0; // 每次游戏开始时清零
        theSeconds = startSeconds;// 设置初始时间
        // 从PlayerPrefs加载金币数据
        coins = PlayerPrefs.GetInt("Coins", 0);
        UpdateScoreUI(); // 更新分数UI
        UpdateCoinUI(); // 更新金币UI

        StartCoroutine(CountdownLoop());// 开始倒计时协程
    }
    // 倒计时协程
    private IEnumerator CountdownLoop()
    {  
        while (theSeconds > 0)
        {
            // 暂停时跳过倒计时
            if (!isPaused)
            {
                theSeconds--;// 只在游戏未暂停时减少时间
                // 更新时间显示
                if (timeText != null)
                    timeText.GetComponent<TMP_Text>().text = "CountDown: " + theSeconds;
            }
            // 等待1秒(使用不受时间缩放影响的时间)
            yield return new WaitForSecondsRealtime(1f); 
        }
        GameOver(); // 时间到，游戏结束
    }
    // 暂停游戏
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;// 停止游戏时间
        pauseUI?.SetActive(true);// 显示暂停UI
        Debug.Log("游戏已暂停");
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 恢复游戏时间
        pauseUI?.SetActive(false); // 隐藏暂停UI
        Debug.Log("游戏已继续");
    }
    // 游戏结束处理
    public void GameOver()
    {
        // 检查并保存最高分
        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScore > best)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
        }
        // 保存金币
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();

       
        FailUI.SetActive(true); // 显示失败UI
                                // 播放失败特效
        if (failEffect != null)
        {
            GameObject effect = Instantiate(failEffect, FailUI.transform.position, Quaternion.identity);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.useUnscaledTime = true;
                Destroy(effect, main.duration);
            }
            else
            {
                Destroy(effect, 1f); // 没粒子系统，1秒后自动销毁
            }
        }
        // 播放失败音效
        if (failSound != null)
        {
            GameObject tempAudio = new GameObject("TempFailAudio");
            AudioSource source = tempAudio.AddComponent<AudioSource>();
            source.clip = failSound;
            source.Play();
            Destroy(tempAudio, failSound.length);
        }

        // 延迟返回主菜单
        StartCoroutine(LoadMenuAfterDelay(2f));
        Time.timeScale = 0f; // 停止游戏时间
    }
    // 延迟加载菜单协程
    private IEnumerator LoadMenuAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // 不受时间缩放影响的等待
        Time.timeScale = 1f; // 恢复时间
        SceneManager.LoadScene("MainMenu"); // 加载主菜单
    }
    // 检查是否可以触发双倍得分
    public bool CanTriggerDoubleScore()
    {
        return !isDoubleScoreActive; // 只有不在双倍状态时才能触发
    }
    // 应用分数加成
    public void ApplyScoreBoost(float multiplier, float duration)
    {
        // 如果已有加成在运行，先停止
        if (doubleScoreRoutine != null)
        {
            StopCoroutine(doubleScoreRoutine);
        }
        // 启动新的加成协程
        doubleScoreRoutine = StartCoroutine(ScoreBoostRoutine(multiplier, duration));
    }
    // 分数加成协程
    private IEnumerator ScoreBoostRoutine(float multiplier, float duration)
    {
        isDoubleScoreActive = true;
        scoreMultiplier = multiplier;

        Debug.Log($"[双倍积分开始] 倍率：{multiplier}，持续：{duration}秒");

        yield return new WaitForSeconds(duration);

        scoreMultiplier = 1f;
        isDoubleScoreActive = false;

        Debug.Log("[双倍积分结束] 恢复为 1 倍");
    }
    // 静态方法 - 增加分数
    public static void AddScore(int value)
    {
        // 应用当前分数乘数
        int finalValue = Mathf.RoundToInt(value * Instance.scoreMultiplier);
        currentScore += finalValue;
        Instance?.UpdateScoreUI(); // 更新UI
    }

    // 静态方法 - 增加金币
    public static void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("Coins", coins); // 立即保存
        Instance?.UpdateCoinUI(); // 更新UI
    }
    // 静态方法 - 消费金币
    public static bool SpendCoins(int amount)
    {
        if (coins >= amount) // 检查是否有足够金币
        {
            coins -= amount;
            PlayerPrefs.SetInt("Coins", coins); // 保存
            Instance?.UpdateCoinUI(); // 更新UI
            return true; // 消费成功
        }

        Debug.Log("金币不足！");
        return false; // 消费失败
    }


    // 获取当前金币数量
    public static int GetCoins() => coins;

    // 更新分数UI
    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    // 更新金币UI
    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "金币：" + coins;
    }
}
