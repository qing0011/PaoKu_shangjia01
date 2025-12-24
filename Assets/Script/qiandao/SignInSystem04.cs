using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInSystem04 : MonoBehaviour
{
    private const string PreviousSignInTime = "PreviousSignInTime";  // 上次签到时间
    private const string SignInCount = "SignInCount";                 // 签到次数

    private DateTime _lastDay;           // 上次签到时间
    private int _signInCount;            // 签到次数
    private const int _maxSignInCount = 7; // 最大签到次数

    private bool _isMaxSignInCount;      // 是否已满签
    private bool _showNextSignInTime;    // 是否显示倒计时

    public Toggle[] _SignInToggleTips;   // UI Toggle
    public Button _signInBtn;            // 签到按钮
    public Text _SignInBtnContent;       // 按钮文字

    
    // 打开界面时执行
    private void OnEnable()
    {
     
        // 读取本地数据
        _lastDay = DateTime.Parse(PlayerPrefs.GetString(PreviousSignInTime, DateTime.MinValue.ToString()));
        _signInCount = PlayerPrefs.GetInt(SignInCount, 0);

        // 判断是否是同一周，不是则重置签到次数
        if (!IsSameWeek(DateTime.Now, _lastDay))
        {
            _signInCount = 0;
            PlayerPrefs.SetInt(SignInCount, _signInCount);
        }

        // 是否已满签
        _isMaxSignInCount = _signInCount >= _maxSignInCount;

        // 刷新UI
        UpdateUI();

        // 判断是否可以签到
        if (CanSignToday())
        {
            _signInBtn.interactable = true;
            _SignInBtnContent.text = "签到";
        }
        else
        {
            _signInBtn.interactable = false;
            _showNextSignInTime = true;
        }
    }

    private void Update()
    {
        // 测试清空数据
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerPrefs.DeleteKey(PreviousSignInTime);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerPrefs.DeleteKey(SignInCount);
        }
        // 如果已满签，禁止一切操作
        if (_isMaxSignInCount) return;

        // 倒计时模式
        if (_showNextSignInTime)
        {
            // 距离今天24点的剩余时间
            TimeSpan interval = DateTime.Now.Date.AddDays(1) - DateTime.Now;
            if (interval > TimeSpan.Zero)
            {
                _SignInBtnContent.text = $"{interval.Hours:D2}:{interval.Minutes:D2}:{interval.Seconds:D2}";
            }
            else
            {
                // 倒计时结束
                _showNextSignInTime = false;
                _signInBtn.interactable = true;
                _SignInBtnContent.text = "签到";
            }
        }

      
    }

    // 签到按钮点击
    public void OnSignInBtnClick()
    {
        // 增加签到次数（不超过最大值）
        _signInCount = Mathf.Min(_signInCount + 1, _maxSignInCount);
        PlayerPrefs.SetInt(SignInCount, _signInCount);

        // 存储本次签到时间
        PlayerPrefs.SetString(PreviousSignInTime, DateTime.Now.ToString());

        // 检查是否满签
        _isMaxSignInCount = _signInCount >= _maxSignInCount;

        // 刷新UI
        UpdateUI();

        // 设置倒计时状态
        if (!_isMaxSignInCount)
        {
            _showNextSignInTime = true;
            _signInBtn.interactable = false;
        }
    }

    // 刷新UI
    private void UpdateUI()
    {
        // 遍历 Toggle 状态
        for (int i = 0; i < _SignInToggleTips.Length; i++)
        {
            _SignInToggleTips[i].isOn = i < _signInCount;
        }

        // 满签后直接禁用按钮
        if (_isMaxSignInCount)
        {
            _signInBtn.interactable = false;
            _SignInBtnContent.text = "已满签";
        }
    }

    // 判断今天是否可以签到
    private bool CanSignToday()
    {
        // 第一次签到
        if (_lastDay == DateTime.MinValue) return true;

        // 是否已满签
        if (_isMaxSignInCount) return false;

        // 判断是否跨天
        return DateTime.Now.Date > _lastDay.Date;
    }

    // 判断是否在同一周
    private bool IsSameWeek(DateTime now, DateTime last)
    {
        if (last == DateTime.MinValue) return true;

        // 获取两者所在周的周一
        DateTime mondayNow = now.AddDays(1 - ((int)now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek)).Date;
        DateTime mondayLast = last.AddDays(1 - ((int)last.DayOfWeek == 0 ? 7 : (int)last.DayOfWeek)).Date;

        return mondayNow == mondayLast;
    }
    
   
}
