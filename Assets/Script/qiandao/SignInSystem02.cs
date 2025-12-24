using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//周几签到周几


//代码逻辑

//存档管理
//使用 PlayerPrefs 保存上次签到时间（字符串格式）。

//签到判断逻辑：1.如果上次签到日期比今天早，就可以领取奖励。
//2.当天只能领取一次奖励，领取后更新上次签到时间。

//UI管理：item 列表存放了一周 7 天的 UI 项（或奖励项）。
//2.调用 IsParticipated(bool) 方法更新该 UI 是否“已签到”。

public class SignInSystem02 : MonoBehaviour
{
    // 存储上次签到时间的Key，用于 PlayerPrefs
    private const string PreviousSignInTime = "PreviousSignInTime";
    //当前日期
    private DateTime _toDay;
    //上次签到日期
    private DateTime _lastDay;
    //当前是星期几
    private int _dayOfWeek;
    //一周中对应的签到奖励UI或者说是数据项列表（Item自定义类或者组件）
    public List<Item2>item = new List<Item2>();
    //当对象启用时调用（场景加载或重新激活时）
    private void OnEnable()
    {
        //当前时间
        _toDay = DateTime.Now;
        //从prefabs获取上次签到时间，如果没有记录则返回DateTime.MinValue
        _lastDay = DateTime.Parse(PlayerPrefs.GetString(PreviousSignInTime, DateTime.MinValue.ToString()));
       //获取当前星期几。返回0表示周日，1表示周一
        _dayOfWeek = (int)_toDay.DayOfWeek;
       //将周日（0）转成7，以便星期一到星期日是1~7
        _dayOfWeek = _dayOfWeek==0 ? 7 : _dayOfWeek;
        //更新签到UI
        UpdateUI();
    }
    //
    private void Update()
    {
        //测试清空签到逻辑：按A清除签到时间,按B清除签到次数
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerPrefs.DeleteAll();
        }
      
    }
    ////更新签到UI
    private void UpdateUI()
    {
        // 根据当前星期几，判断当天奖励是否已领取
        // IsParticipated(true) 表示已签到，false 表示可签到
        bool canGet = CanGetReward();
        Debug.Log($"当前星期：{_dayOfWeek}，是否可领：{canGet}");
        item[_dayOfWeek - 1].IsParticipated(!CanGetReward());
    }
    // 当点击“领取奖励”按钮时调用
    public void OnGetRewardBtnClick()
    {
        //领取奖励
        _toDay = DateTime.Now; // 刷新当前时间
        // 记录签到时间为今天
        _lastDay = _toDay;
        // 将签到时间保存到 PlayerPrefs
        PlayerPrefs.SetString(PreviousSignInTime, _lastDay.ToString());
        Debug.Log("领取奖励，更新时间：" + _lastDay);
        UpdateUI();
    }
    // 判断今天是否可以领取奖励
    private bool CanGetReward()
    {
        // 判断条件：上次签到时间必须早于今天
        // 只要日期小于今天即可签到（不考虑跨月、跨年的复杂情况）
        if (_lastDay.Year <= _toDay.Year && _lastDay.Month <= _toDay.Month && _lastDay.Day < _toDay.Day)
        {
            return true;
        }
        return false;

    }

}
