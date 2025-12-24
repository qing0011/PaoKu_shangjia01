using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

//左右换道、自动前进、跳跃、滑铲、加速


public class PlayerMovement : MonoBehaviour
{
    [Header("移动控制")]
    //定义玩家的基本移动属性和检测参数。
    //LayerMask 用来限制检测只作用于地面（防止误判其他物体）。
    [SerializeField] private float laneWidth = 2f;       // 跑道间距
    [SerializeField] private float laneSwitchSpeed = 8f; // 跑道切换速度
    [SerializeField] private float forwardSpeed = 10f;   // 自动前进速度
    [SerializeField] private float jumpForce = 5f;       // 跳跃力度
    [SerializeField] private LayerMask groundLayer;      // 地面层级（在Inspector中设置）
    [SerializeField] private float groundCheckDistance = 0.2f; // 地面检测距离
    [Header("滑铲控制")]
    //设置滑铲时碰撞体形状的变化，用来模拟玩家滑行趴下。
    [SerializeField] private float slideDuration = 1f; // 滑铲持续时间
    [SerializeField] private float slideColliderHeight = 1f; // 滑铲时的碰撞体高度
    [SerializeField] private float slideColliderCenterY = 0.5f; // 滑铲时的碰撞体中心Y
    ///<summary>
    /// 组件变量 ，状态变量
    /// </summary>
    private int currentLane = 1; // 当前跑道索引（0左, 1中, 2右）
    //角色的刚体动画和碰撞
    private Rigidbody rb;
    private Animator animator;
    private CapsuleCollider capsule;
    //滑铲过程控制变量，记录滑铲是否进行中、剩余时间，并保存滑铲前的碰撞体原始参数，用于恢复
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    //做加速器
    /// <summary>
    private float baseSpeed; // 原始基础速度
    private float currentSpeed;//是当前实际速度（加速期间变快）
    private Coroutine speedBoostRoutine;//用于控制协程

    void Start()
    {
        Physics.gravity = new Vector3(0, -15f, 0); // 增强重力使跳跃更有落地感
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();//获取所需组件，记录初始值，修改全局重力。
        //加速器
        baseSpeed = forwardSpeed;
        currentSpeed = baseSpeed;
       
        capsule = GetComponent<CapsuleCollider>();

        if (capsule == null)
        {
            Debug.LogError("PlayerMovement 警告：缺少 CapsuleCollider 组件！");
        }
        rb.freezeRotation = true;
        //保存胶囊碰撞体的原始高度和中心位置，为后续滑铲恢复使用。
        if (capsule != null)
        {
            originalColliderHeight = capsule.height;
            originalColliderCenter = capsule.center;
        }
    }
    void Update()
    {
        //处理输入、滑铲更新
        HandleInput();//每帧检测键盘输入
        UpdateSlide();//检查滑铲时间是否结束，并恢复站立状态
    }
    void FixedUpdate()
    {
        MoveForward();//在物理帧中调用向前移动方法。
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red); // 使用射线可视化地面检测
    }
    //键盘输入
    //左右键切换跑道： 向上键触发跳跃（需地面、未冷却）。。向下键触发滑铲（需地面、未正在滑）
    void HandleInput()
    {
        // 左右切换跑道
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            SwitchLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            SwitchLane(1);
        }
        // 跳跃（使用射线检测地面）
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && IsGrounded())
        {
            Jump();

        }
        //滑铲
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && IsGrounded() && !isSliding)
        {
            StartSlide();
        }
    }
    /// <summary>
    //加速器逻辑
    //只能在地面时生效；
    //使用协程控制加速持续时间。
    /// <summary>
    public void ApplySpeedBoost(float multiplier, float duration)//在玩家碰到加速道具时触发此方法
    {
        if (!IsGrounded()) return; // 禁止空中加速
        Debug.Log($"[加速器触发] 当前速度倍率: {multiplier}，持续时间: {duration}秒");
        // 如果已有加速协程，先停止它
        if (speedBoostRoutine != null) StopCoroutine(speedBoostRoutine);

        speedBoostRoutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }
    //临时修改 currentSpeed 实现加速，并在一定时间后恢复。
    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        currentSpeed = baseSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        currentSpeed = forwardSpeed;
        speedBoostRoutine = null;
    }
    //辅助逻辑：跳跃冷却。。防止连续跳跃，加入冷却时间。

    private bool isJumping = false;
    IEnumerator JumpCooldown(float time)
    {
        isJumping = true;
        yield return new WaitForSeconds(time);
        isJumping = false;
    }
    //换道逻辑：防止换道越界（只允许 0-2 三条跑道）。
    void SwitchLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, 0, 2);
    }
    /// <summary>
    /// 移动 + 跳跃 + 滑铲 动作实现
    /// </summary>
    //前进
    void MoveForward()
    {
        //前进速度为 currentSpeed（可加速）：左右移动用 Lerp 平滑过渡
        float targetX = (currentLane - 1) * laneWidth;
        Vector3 targetPos = new Vector3(
            targetX,
            rb.position.y,
            //rb.position.z + forwardSpeed * Time.fixedDeltaTime
            rb.position.z + currentSpeed * Time.fixedDeltaTime

        );
        rb.MovePosition(Vector3.Lerp(rb.position, targetPos, laneSwitchSpeed * Time.fixedDeltaTime));
    }
    //跳跃
    void Jump()
    {
        //设置刚体 Y 方向速度实现跳跃； 播放动画；
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z); // 重置Y轴速度避免叠加力
        animator.SetTrigger("Jump"); // 改回使用触发器
        // 添加一个小延迟防止立即检测地面
        StartCoroutine(DisableGroundCheckBriefly());
    }
    //短暂禁用地面检测避免立即被判定为落地。
    IEnumerator DisableGroundCheckBriefly()
    {
        var originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // 改为忽略射线检测的层级
        yield return new WaitForSeconds(0.1f);
        gameObject.layer = originalLayer;
    }
    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        // 修改碰撞体参数
        capsule.height = slideColliderHeight;
        capsule.center = new Vector3(capsule.center.x, slideColliderCenterY, capsule.center.z);

        animator.SetTrigger("Slider");
    }
    void UpdateSlide()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }
    }

    void EndSlide()
    {
        if (capsule == null) return;
        isSliding = false;
        // 恢复碰撞体参数
        capsule.height = originalColliderHeight;
        capsule.center = originalColliderCenter;
    }
    void ShowGameOverUI()
    {
        PlayerStats.Instance.GameOver();
        // 你可以在这里调用 UI 管理器显示失败界面
        Debug.Log("Game Over!");
    }

    //用 Raycast 精准检测玩家是否站在地面上。
    bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        return Physics.Raycast(ray, groundCheckDistance + 0.1f, groundLayer);
    }
    // 调试用：可视化碰撞体
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, 0.1f);
    }
}