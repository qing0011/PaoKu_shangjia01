using UnityEngine;

public class SpeedBoostItem : MonoBehaviour
{
    [Header("加速设置")]
    public float speedMultiplier = 2f;   // 加速倍率
    public float duration = 5f;           // 持续时间（秒）

    [Header("效果")]
    public GameObject pickupEffectPrefab; // 拾取特效（可选）
    public AudioClip pickupSound;          // 拾取声音（可选）

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
        {
            // 如果没有AudioSource组件但有声音，则自动添加一个
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // 调用加速器
                playerMovement.ApplySpeedBoost(speedMultiplier, duration);
            }

            // 播放特效
            if (pickupEffectPrefab != null)
            {
                Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            }

            // 播放声音
            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            // 销毁道具物体（如果播放声音需要等声音播完可改成延迟销毁）
            Destroy(gameObject);
        }
    }
}
