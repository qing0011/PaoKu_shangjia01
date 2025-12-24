using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public GameObject explosionEffect; //障碍物 特效预制体
    public AudioClip explosionSound;   // 音效

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Explosion triggered");

            // 启动协程等待特效和音效播放完再死亡
            StartCoroutine(HandleExplosionSequence(other));
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
        float waitTime = Mathf.Max(effectDuration, soundDuration);
        yield return new WaitForSeconds(waitTime);

        // 4. 播放完后让玩家死亡
        PlayerDeath playerDeath = other.GetComponent<PlayerDeath>();
        if (playerDeath != null)
        {
            playerDeath.Die();
        }
    }
}
