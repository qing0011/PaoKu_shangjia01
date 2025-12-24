using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // 播放死亡动画（可选）
        if (animator != null)
            animator.SetTrigger("Die");

        // 禁用玩家控制
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        // 正确触发 GameOver 流程（暂停 + 显示UI + 延迟返回菜单）
        PlayerStats.Instance.GameOver();
    }

    void ShowGameOverUI()
    {
       
        // 实现你的失败界面显示逻辑
        Debug.Log("Game Over!");
    }
}
