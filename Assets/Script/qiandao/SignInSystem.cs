using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//功能          说明
//本地保存  	使用 PlayerPrefs 保存签到日期和次数
//签到次数上限  	超过 7 次将重置
//倒计时显示	 未到第二天则显示时间倒计时
//UI更新	   根据签到次数更新 Toggle 状态
//签到按钮	    自动切换“签到”或“倒计时”显示


// 签到系统控制脚本
public class SignInSystem : MonoBehaviour
{
    // 存储上次签到时间的Key
    private const string PreviousSignInTime = "PreviousSignInTime";
    // 存储签到次数的Key
    private const string SignInCount = "SignInCount";

    private DateTime _toDay;//当前签到时间
    private DateTime _lastDay;//上次签到时间
    private TimeSpan _interval;//时间间隔

    private int _signInCount;//签到次数
    private int _maxSignInCount=7;//最大可签到数

    private bool _showNextSginInTime;//显示下一次签到时间
    private bool _isMaxSignInCount;//达到最大签到数

    public Toggle[] _SignInToggleTips;//是否已经签到（打勾表示已签到）
    public Button _signInBtn;//签到按钮
    public Text _SignInBtnContent;//显示时间（签到按钮上的文字）

    // 每次界面启用时执行（如进入签到页面）
    private void OnEnable()
    {

        _toDay = DateTime.Now;//获得当前时间
        //从本地读取上次签到时间，没有就设置为最小时间
        _lastDay = DateTime.Parse(PlayerPrefs.GetString(PreviousSignInTime, DateTime.MinValue.ToString()));
       //从本地读取当前签到次数
        _signInCount = PlayerPrefs.GetInt(SignInCount, 0);
        //循环签到逻辑。如果签到次数达到最大，就清空重新开始
        if (_signInCount >= _maxSignInCount)
        {
            PlayerPrefs.DeleteKey(SignInCount);
            _signInCount = 0;
        }
        //刷新UI
        UpdateUI();

        if (IsCanSignIn())
        {
            //可以签到// 可以签到：启用按钮、显示“签到”
            _signInBtn.interactable = true;
            _SignInBtnContent.text = "签到";
        }
        else
        {
            //不可以签到:禁用按钮、开始显示倒计时
            _showNextSginInTime = true;
            _signInBtn.interactable = false;
        }
    }
    // 每帧调用（倒计时刷新）
    private void Update()
    {
        //测试清空签到逻辑：按A清除签到时间,按B清除签到次数
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerPrefs.DeleteKey(PreviousSignInTime);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerPrefs.DeleteKey(SignInCount);
        }

        // 如果已经满签，就什么都不做
        if (_isMaxSignInCount)
        {
           return;
        }
        // 如果正在等待下一次签到
        if (_showNextSginInTime)
        {
            // 计算距离第二天零点的时间差
            _interval = _toDay.AddDays(1).Date - DateTime.Now;
            //间隔时间一定是大于等于0..//// 只有在时间差为正时才更新倒计时
            if (_interval >= TimeSpan.Zero)
            {
                _SignInBtnContent.text =$"{_interval.Hours}:{_interval.Minutes}:{_interval.Seconds}" ;
            }
            else
            {
                //可以签到: // 倒计时结束，按钮变为可点击，显示“签到”
                _signInBtn.interactable = true;
                _SignInBtnContent.text = "签到";
                _showNextSginInTime = false;
                
            }
        }
    }
    // 签到按钮点击事件
    public void OnSignInBtnClick()
    {
        //给奖励
        // 增加签到次数
        _signInCount++;
        // 存储本次签到时间
        PlayerPrefs.SetString(PreviousSignInTime, DateTime.Now.ToString());
        PlayerPrefs.SetInt(SignInCount, _signInCount);
        // 设置按钮为不可点击并显示倒计时
        _showNextSginInTime = true;
        _signInBtn.interactable = false ;
        //手动刷新UI
        UpdateUI();
       // IsCanSignIn();
    }
    //是否可以签到，
    private bool IsCanSignIn()
    {
        ////如果达到最大签到数一定不可以签到
        //if(_signInCount >= _maxSignInCount)
        //{
        //    _signInBtn.interactable = false;
        //    _SignInBtnContent.text = "达到最大签到数";
        //    _isMaxSignInCount = true;
        //    return false;
        //}

        //年月一定是小于等于当前的年月，日肯定是小于当前的日
        // 判断日期是否已经过了一天（同年、同月且“上次签到日” < “今天”）
        if (_lastDay.Year<=_toDay.Year && _lastDay.Month<=_toDay.Month && _lastDay.Day < _toDay.Day)
        {
           return true;
        }
        return false;
    }
    // 刷新签到UI显示
     private void UpdateUI()
    {
      
        // 确保不超出数组范围
       
        for (int i=0; i < _signInCount; i++)
        {
            _SignInToggleTips[i].isOn = true;
        }

    }


 

}
