using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//一次性签到

namespace A
{


public class SignInSystem : MonoBehaviour
{
    private const string PreviousSignInTime = "PreviousSignInTime";
    private const string SignInCount = "SignInCount";

    private DateTime _toDay;
    private DateTime _lastDay;
    private TimeSpan _interval;//时间间隔

    private int _signInCount;//签到次数
    private int _maxSignInCount = 7;//最大可签到数

    private bool _showNextSginInTime;//显示下一次签到时间
    private bool _isMaxSignInCount;//达到最大签到数

    public Toggle[] _SignInToggleTips;//是否已经签到
    public Button _signInBtn;//签到按钮
    public Text _SignInBtnContent;//显示时间


    private void OnEnable()
    {
        _toDay = DateTime.Now;
        _lastDay = DateTime.Parse(PlayerPrefs.GetString(PreviousSignInTime, DateTime.MinValue.ToString()));
        _signInCount = PlayerPrefs.GetInt(SignInCount, 0);
            //做循环签到的逻辑
        if (_signInCount >= _maxSignInCount)
            {
                PlayerPrefs.DeleteKey(SignInCount);
                _signInCount = 0;
            }
        //结束
            UpdateUI();

          

        if (IsCanSignIn())
        {
            //可以签到
            _signInBtn.interactable = true;
            _SignInBtnContent.text = "签到";
        }
        else
        {
            //不可以签到
            _showNextSginInTime = true;
            _signInBtn.interactable = false;
        }
    }
    private void Update()
    {
        //测试清空签到逻辑
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerPrefs.DeleteKey(PreviousSignInTime);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerPrefs.DeleteKey(SignInCount);
        }


        if (_isMaxSignInCount)
        {
            return;
        }
        if (_showNextSginInTime)
        {
            _interval = _toDay.AddDays(1).Date - DateTime.Now;
            //间隔时间一定是大于等于0
            if (_interval >= TimeSpan.Zero)
            {
                _SignInBtnContent.text = $"{_interval.Hours}:{_interval.Minutes}:{_interval.Seconds}";
            }
            else
            {
                //可以签到
                _signInBtn.interactable = true;
                _SignInBtnContent.text = "签到";
                _showNextSginInTime = false;

            }
        }
    }
    public void OnSignInBtnClick()
    {
        //给奖励
        _signInCount++;
        PlayerPrefs.SetString(PreviousSignInTime, DateTime.Now.ToString());
        PlayerPrefs.SetInt(SignInCount, _signInCount);
        _showNextSginInTime = true;
        _signInBtn.interactable = false;
        //手动刷新UI
        UpdateUI();
        IsCanSignIn();
    }
    //是否可以签到，
    private bool IsCanSignIn()
    {
        //如果达到最大签到数一定不可以签到
        //if (_signInCount >= _maxSignInCount)
        //{
        //    _signInBtn.interactable = false;
        //    _SignInBtnContent.text = "达到最大签到数";
        //    _isMaxSignInCount = true;
        //    return false;
        //}

        //年月一定是小于等于当前的年月，日肯定是小于当前的日
        if (_lastDay.Year <= _toDay.Year && _lastDay.Month <= _toDay.Month && _lastDay.Day < _toDay.Day)
        {
            return true;
        }
        return false;
    }
    private void UpdateUI()
    {

        // 确保不超出数组范围

        for (int i = 0; i < _signInCount; i++)
        {
            _SignInToggleTips[i].isOn = true;
        }

    }


    //private void Start()
    //{
    //    //DateTime a1 = DateTime.Now;
    //    //DateTime a2 = DateTime. UtcNow;
    //    //Debug.Log(a1);
    //    //Debug.Log(a2);
    //    //long a = LocalTimeToTimeStamp(a1);
    //    //long b = LocalTimeToTimeStamp(a2);
    //    //Debug.Log(TimeStampToLocalTime(a));
    //    //Debug.Log(TimeStampToLocalTime(b));
    //}

    //本地时间转时间戳
    //private long LocalTimeToTimeStamp(DateTime locaTime)
    //{
    //    //2025
    //    return ((DateTimeOffset)locaTime).ToUnixTimeSeconds();
    //}
    ////时间戳转时间
    //private DateTime TimeStampToLocalTime(long timeStamp)
    //{
    //    //2025+timeStamp秒数 To 本地时间
    //    return DateTime.UnixEpoch.AddSeconds(timeStamp).ToLocalTime();
    //}



}
}
