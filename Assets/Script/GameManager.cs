using UnityEngine;
using System.Collections;
// GameManager类 - 游戏主控制器，管理游戏流程和全局状态
public class GameManager : MonoBehaviour
{
    // 单例模式实例，方便其他脚本访问
    public static GameManager Instance;

    [Header("障碍物控制器")]
    public ObstacleSpawner obstacleSpawner;// 引用障碍物生成器脚本

    [Header("定时刷特殊道具设置")]
    public float initialDelay = 30f;    // 首次生成特殊道具的延迟时间(秒)
    public float repeatInterval = 5f;   // 之后生成特殊道具的时间间隔(秒)

    // 协程引用
    private Coroutine timedSpawnRoutine;    // 定时生成协程
    //private Coroutine specialItemRoutine;  // 特殊道具协程

    // Awake方法 - 在脚本初始化时调用
    private void Awake()
    {
        // 设置单例实例
        Instance = this;
    }
    // Start方法 - 在游戏开始时调用
    private void Start()
    {
        // 启动定时生成特殊道具的协程
        timedSpawnRoutine = StartCoroutine(SpawnTimedItemRoutine());
    }
    // 定时生成特殊道具的协程
    private IEnumerator SpawnTimedItemRoutine()
    {
        // 等待初始延迟时间
        yield return new WaitForSeconds(initialDelay);
        // 无限循环生成特殊道具
        while (true)
        {
            // 调用障碍物生成器的概率生成方法
            obstacleSpawner.SpawnWithProbability();
            // 等待间隔时间
            yield return new WaitForSeconds(repeatInterval);
        }
    }
    // 玩家死亡处理方法
    public void PlayerDie()
    {
        // 调用玩家状态统计的游戏结束方法
        PlayerStats.Instance.GameOver();
    }
}
