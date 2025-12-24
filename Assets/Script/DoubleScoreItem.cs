using UnityEngine;

public class DoubleScoreItem : MonoBehaviour
{
    public float duration = 5f;     // 持续时间
    public float multiplier = 2f;    // 倍率
    public GameObject effectPrefab;  // 拾取特效
    public AudioSource collectAudioSource;   // 拾取音效（可选）

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats stats = PlayerStats.Instance;
        if (stats == null) return;

        if (stats.CanTriggerDoubleScore())
        {
            stats.ApplyScoreBoost(multiplier, duration);

            if (collectAudioSource != null)
                collectAudioSource.Play();

            if (effectPrefab != null)
                Instantiate(effectPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("已处于双倍积分状态，忽略重复拾取");
            // 可选销毁或保留道具
            Destroy(gameObject);
        }
    }
}
