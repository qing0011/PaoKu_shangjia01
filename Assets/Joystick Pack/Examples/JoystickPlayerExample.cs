using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这是一个使用虚拟摇杆控制物体移动的示例脚本
public class JoystickPlayerExample : MonoBehaviour
{
    public float speed; // 移动速度系数，可在Unity编辑器中调整

    // 引用VariableJoystick组件，用于获取摇杆输入
    public VariableJoystick variableJoystick;

    // 引用Rigidbody组件，用于物理移动
    public Rigidbody rb;

    // FixedUpdate在固定时间间隔调用，适合物理计算
    public void FixedUpdate()
    {
        // 根据摇杆输入计算移动方向:
        // - Vertical控制前后(Z轴)移动
        // - Horizontal控制左右(X轴)移动
        Vector3 direction = Vector3.forward * variableJoystick.Vertical
                          + Vector3.right * variableJoystick.Horizontal;

        // 对刚体施加力实现移动:
        // - direction * speed 决定力的大小和方向
        // - Time.fixedDeltaTime 确保帧率不影响移动速度
        // - ForceMode.VelocityChange 直接改变速度，忽略质量
        rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}