using UnityEngine;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour
{
    [Header("跑道生成设置")]
    [SerializeField] private GameObject initialGroundPrefab; // 初始固定地面
    [SerializeField] private GameObject[] trackPrefabs;
    [SerializeField] private float trackLength = 50f;
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private Transform player;

    private List<GameObject> activeTracks = new List<GameObject>();
    private float nextSpawnPosZ = 0f;//记录下一段跑道的生成位置

    [Header("障碍物生成器")]
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    [SerializeField] private float destroyDelay = 2f; // 延迟销毁时间（秒）
    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        // 首先生成固定的初始地面
        SpawnInitialGround();
        nextSpawnPosZ += trackLength;
        for (int i = 0; i < 3; i++)
        {
            SpawnTrack(nextSpawnPosZ);
            nextSpawnPosZ += trackLength;
        }
    }

    private void Update()
    {
        if (player.position.z > activeTracks[activeTracks.Count - 1].transform.position.z - spawnDistance )
        {
            SpawnTrack(nextSpawnPosZ);
            nextSpawnPosZ += trackLength;
        }

        if (activeTracks.Count > 0 &&
            player.position.z > activeTracks[0].transform.position.z + trackLength)
        {
            // 延迟销毁第一个跑道
            Destroy(activeTracks[0],destroyDelay);
            activeTracks.RemoveAt(0);
        }
    }
    private void SpawnInitialGround()
    {
        GameObject newTrack = Instantiate(
            initialGroundPrefab,
            new Vector3(0, 0, nextSpawnPosZ),
            Quaternion.identity);

        activeTracks.Add(newTrack);

        // 初始地面可以不需要障碍物，或者你也可以保留这部分
        obstacleSpawner.SetTrack(newTrack, trackLength);
        obstacleSpawner.SpawnWithProbability();
    }
    private void SpawnTrack(float zPosition)
    {
        GameObject newTrack = Instantiate(
            trackPrefabs[Random.Range(0, trackPrefabs.Length)],
            new Vector3(0, 0, zPosition),
            Quaternion.identity);

        activeTracks.Add(newTrack);

        obstacleSpawner.SetTrack(newTrack, trackLength);
        obstacleSpawner.SpawnWithProbability();
    }
}
