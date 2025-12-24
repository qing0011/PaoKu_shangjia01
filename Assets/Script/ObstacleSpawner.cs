using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    // 道具预制体数组
    [Header("道具预制体")]
    [SerializeField] private GameObject[] commonObstaclePrefabs; // 普通障碍物预制体
    [SerializeField] private GameObject[] functionalItemPrefabs; // 功能道具预制体
    [SerializeField] private GameObject[] specialItemPrefabs; // 特殊道具预制体

    // 生成概率设置
    [Header("生成设置")]
    [SerializeField] private int totalSpawnCount = 20; // 总生成数量
    [Range(0f, 1f)] public float commonPercent = 0.6f; // 普通障碍物生成概率
    [Range(0f, 1f)] public float functionalPercent = 0.3f; // 功能道具生成概率
    [Range(0f, 1f)] public float specialPercent = 0.1f; // 特殊道具生成概率

    // 生成位置参数
    [Header("生成范围")]
    [SerializeField] private float trackHeight = 1.2f; // 生成高度
    [SerializeField] private float edgeZPadding = 3f; // 轨道两端留空距离
    [SerializeField] private float minDistance = 5f; // 道具间最小间距

    // 车道设置
    [Header("车道设置")]
   // [SerializeField] private float laneOffset = 2f; // 车道偏移量
    [SerializeField] private float laneSpacing = 2f; // 直接定义车道间隔（如2米）
    private float[] lanePositions; // 存储各车道位置

    private GameObject currentTrack; // 当前轨道对象
    private float currentTrackLength; // 当前轨道长度
    private bool onlySpawnSpecial = false; // 是否只生成特殊道具

    private void Awake()
    {
        // 计算车道位置：左(-spacing/2)，中(0)，右(+spacing/2)
        lanePositions = new float[] { -laneSpacing , 0f, laneSpacing  };
    }
    /// <summary>
    /// 设置当前轨道和轨道长度
    /// </summary>
    /// <param name="track">轨道游戏对象</param>
    /// <param name="length">轨道长度</param>
    public void SetTrack(GameObject track, float length)
    {
        currentTrack = track;
        currentTrackLength = length;
    }
    /// <summary>
    /// 设置是否只生成特殊道具
    /// </summary>
    /// <param name="value">true表示只生成特殊道具</param>
    public void SetOnlySpawnSpecial(bool value)
    {
        onlySpawnSpecial = value;
    }
    /// <summary>
    /// 根据概率生成道具
    /// </summary>
    public void SpawnWithProbability()
    {
        // 安全检查当前轨道是否存在
        if (currentTrack == null || currentTrack.Equals(null)) return;
        // 循环生成指定数量的道具
        for (int i = 0; i < totalSpawnCount; i++)
        {
            GameObject[] selectedArray = null;
            string tag = "";
            // 检查是否只生成特殊道具
            if (onlySpawnSpecial)
            {
                selectedArray = specialItemPrefabs;
                tag = "SpecialItem";
            }
            else
            {
                // 根据概率随机选择道具类型
                float rand = Random.value;
                float commonThreshold = commonPercent;
                float functionalThreshold = commonPercent + functionalPercent;

                if (rand < commonPercent)
                {
                    selectedArray = commonObstaclePrefabs;
                    tag = "Obstacle";
                }
                else if (rand < commonPercent + functionalPercent)
                {
                    selectedArray = functionalItemPrefabs;
                    tag = "FunctionalItem";
                }
                else
                {
                    selectedArray = specialItemPrefabs;
                    tag = "SpecialItem";
                }
            }
            // 检查选择的道具数组是否有效
            if (selectedArray == null || selectedArray.Length == 0) continue;
             // 从数组中随机选择一个预制体
            Vector3 pos = GetValidPosition();
            // 从数组中随机选择一个预制体
            GameObject prefab = selectedArray[Random.Range(0, selectedArray.Length)];
            // 安全检查 prefab
            if (prefab == null) continue;

            // 再次确认 currentTrack 未被销毁
            if (currentTrack == null || currentTrack.Equals(null)) return;
            // 实例化道具并设置位置和标签
            GameObject obj = Instantiate(prefab, currentTrack.transform);
            obj.transform.localPosition = pos;
            obj.tag = tag;
        }
    }
    /// <summary>
    /// 清除轨道上的所有道具
    /// </summary>
    public void ClearItems()
    {
        if (currentTrack == null) return;
        // 遍历轨道所有子物体，销毁道具
        foreach (Transform child in currentTrack.transform)
        {
            if (child.CompareTag("FunctionalItem") || child.CompareTag("Obstacle"))
                Destroy(child.gameObject);
        }
    }
    /// <summary>
    /// 获取有效的生成位置
    /// </summary>
    /// <returns>有效的生成位置</returns>
    private Vector3 GetValidPosition()
    {
        Vector3 pos;
        int attempts = 0;
        bool valid;

        do
        {
            // 随机选择车道和轨道上的位置
            float x = lanePositions[Random.Range(0, lanePositions.Length)];
            float z = Random.Range(edgeZPadding, currentTrackLength - edgeZPadding);

            pos = new Vector3(x, trackHeight, z);
            // 检查是否与其他道具距离过近
            //valid = true;
            //foreach (Transform child in currentTrack.transform)
            //{
            //    if (Vector3.Distance(child.localPosition, pos) < minDistance)
            //    {
            //        valid = false;
            //        break;
            //    }
            //}
            // 以球体方式检测是否重叠，半径可调
            float checkRadius = minDistance / 2f;
            Collider[] colliders = Physics.OverlapSphere(currentTrack.transform.TransformPoint(pos), checkRadius);

            // 是否有其他障碍物/道具存在
            valid = colliders.Length == 0;

            attempts++;
            // 防止无限循环，尝试10次后强制返回当前位置
            if (attempts > 10) valid = true;

        } while (!valid);

        return pos;
    }
}
