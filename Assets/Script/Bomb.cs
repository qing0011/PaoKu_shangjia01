using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect; // 爆炸特效预制体
    public AudioClip explosionSound;   // 爆炸音效

    private bool hasExploded = false; // 防止多次触发

    private void OnTriggerEnter(Collider other)
    {
        //防止多次触发
        if (hasExploded) return;

        if (other.CompareTag("Player"))
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Explosion triggered");

                // 启动协程等待特效和音效播放完再死亡
                StartCoroutine(HandleExplosionSequence(other));
            }
        }
    }
   
        private IEnumerator HandleExplosionSequence(Collider other)
        {
            float effectDuration = 0f;
            float soundDuration = 0f;
            // 1. 碰撞瞬间就暂停玩家移动
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;
            //  2.停止玩家动画，比如奔跑动画
            Animator anim = other.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("Die");
            // 1. 播放爆炸特效
            if (explosionEffect != null)
            {
                GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.useUnscaledTime = true; // 不受 Time.timeScale 影响
                    effectDuration = main.duration;
                    Destroy(effect, effectDuration);
                }
                else
                {
                    // 没有粒子系统就设个默认时间
                    effectDuration = 0.1f;
                }
            }

            // 2. 播放爆炸声音
            if (explosionSound != null)
            {
                GameObject tempAudio = new GameObject("TempAudio");
                AudioSource source = tempAudio.AddComponent<AudioSource>();
                source.clip = explosionSound;
                source.Play();
                soundDuration = explosionSound.length;
                Destroy(tempAudio, soundDuration);
            }

            // 3. 等待最长的那个时长
           // float waitTime = Mathf.Max(effectDuration, soundDuration);
            yield return new WaitForSeconds(0.5f);

            // 4. 播放完后让玩家死亡
            PlayerDeath playerDeath = other.GetComponent<PlayerDeath>();
            if (playerDeath != null)
            {
                playerDeath.Die();
            }
        }
       
}

