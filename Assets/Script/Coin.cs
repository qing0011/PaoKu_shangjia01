using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Coin : MonoBehaviour
{
    [Header("金币得分")]
    public int score = 1000;  // 在 Inspector 中手动设置

    [Header("金币音效")]
    public AudioClip collectSound;
    [Header("金币缩放范围")]
    public float minScale = 3f;   // 最小缩放
    public float maxScale = 5f;   // 最大缩放
    [Header("特效")]
    public GameObject effectPrefab;  // 金币特效
    [Header("未采集金币消失")]
    public float autoDestroyDistance = 10f; // 玩家超过金币多少距离就销毁

    private Transform playerTransform;

    void Start()
    {
        // 在游戏开始时对金币进行一次随机缩放
        float scale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(scale, scale, scale);
        //未采集金币消失
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    IEnumerator Collect()
    {
        if (effectPrefab != null)
            Instantiate(effectPrefab, transform.position, Quaternion.identity);

        //// 隐藏金币显示（可选）
        //GetComponent<MeshRenderer>().enabled = false;
        //GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(0.1f); // 等待特效播放完成

        Destroy(gameObject);
    }
   ///未采集金币消失
    void Update()
    {
        // 玩家向前移动（例如 Z 轴），金币在玩家后方，则销毁
        if (playerTransform != null && playerTransform.position.z - transform.position.z > autoDestroyDistance)
        {
            Destroy(gameObject);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.AddScore(score);

            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            StartCoroutine(Collect());
        }
    }



}
