using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 每周顺序签到系统：
/// - 只能从第一个未签的天开始签到（顺序补签）
/// - 跨周自动重置（周一为一周起点）
/// - 默认 24 小时冷却（可切换为跨 0 点重置）
/// - UI：已签置灰、当前可签高亮、其余锁定置灰
/// 依赖 Item：IsParticipated(bool), SetInteractable(bool)
/// </summary>


public class SignInSystem03 : MonoBehaviour
{
    // 存储上次签到时间的Key，用于 PlayerPrefs
    private const string PreviousSignInTime = "PreviousSignInTime";
    // 存储签到次数的Key
    private const string SignInCount = "SignInCount";

    //当前日期
    private DateTime _toDay;
    //上次签到日期
    private DateTime _lastDay;
    //签到次数
    int _signInCount;

    //一周中对应的签到奖励UI或者说是数据项列表（Item自定义类或者组件）
    public List<Item>item = new List<Item>();



    //当对象启用时调用（场景加载或重新激活时）
    private void OnEnable()
    {
        
        //当前时间
        _toDay = DateTime.Now;
        //从prefabs获取上次签到时间，如果没有记录则返回DateTime.MinValue
        _lastDay = DateTime.Parse(PlayerPrefs.GetString(PreviousSignInTime, DateTime.MinValue.ToString()));
       
        //从本地读取当前签到次数
        _signInCount = PlayerPrefs.GetInt(SignInCount, 0);
        //判断上一次签到的时间和这一次是否是同一周，不是的话就清空
        if(!IsOneWeek())
        {
            _signInCount = 0;
            PlayerPrefs.SetInt(SignInCount,_signInCount);
        }
        //更新签到UI
        UpdateUI();
    }
    private void Update()
    {
        //测试清空签到逻辑：按A清除签到时间,按B清除签到次数
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerPrefs.DeleteAll();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerPrefs.DeleteKey(SignInCount);
        }
    }
    // 点击签到按钮
    public void OnGetRewardBtnClick()
    {
        if (!CanSignToday()) return; // 冷却中不允许签到

        // 签到次数+1（最多7天）
        _signInCount = Mathf.Min(_signInCount + 1, 7);

        // 更新签到时间
        _lastDay = DateTime.Now;

        // 保存数据
        PlayerPrefs.SetInt(SignInCount, _signInCount);
        //
        PlayerPrefs.SetString(PreviousSignInTime, _lastDay.ToString());

        UpdateUI();
    }
    /// <summary>
    /// 更新所有签到按钮的状态显示
    /// 逻辑：
    /// - 已签到的按钮置灰且不可点击
    /// - 当前第一个未签到的按钮（索引为_signInCount）根据是否冷却完成决定是否可点击
    /// - 后续按钮均置灰不可点
    /// </summary>
    private void UpdateUI() 
    {
        // 获取今天是周几
        int dayOfWeek = (int)_toDay.DayOfWeek;
        dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
        // 遍历所有签到按钮
        for (int i = 0; i < item.Count; i++)
        {
            // 已签到的按钮设置成已参与，且不可交互
            if (i < _signInCount)
            {
                // 已签到
                item[i].IsParticipated(true);
                item[i].SetInteractable(false);
            }
            else if (i == _signInCount)
            {
                // 当前可签到的第一个按钮
                if (CanSignToday())
                {
                    item[i].IsParticipated(false);
                    item[i].SetInteractable(true); // 高亮可点击
                }
                else
                {
                    item[i].IsParticipated(false);
                    item[i].SetInteractable(false); // 24小时未到，灰色
                }
            }
            else
            {
                // 后续日期：未到，灰色禁用
                item[i].IsParticipated(false);
                item[i].SetInteractable(false);
            }
        }
       
    }
    /// <summary>
    /// 判断是否可以签到（冷却时间是否到）
    /// 规则：如果没签到过，允许签到；否则必须距离上次签到超过24小时
    /// </summary>
    private bool CanSignToday()
    {
        // 如果没有签到过，直接可以签到
        if (_lastDay == DateTime.MinValue) return true;

        // 判断是否超过24小时
        return DateTime.Now >= _lastDay.AddHours(24);
    }
   
    private bool IsOneDay()
    {
        if (_lastDay.Year <= _toDay.Year && _lastDay.Month <= _toDay.Month && _lastDay.Day < _toDay.Day)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 判断是否是同一周（以周一为一周的第一天）
    /// 通过比较两日期所在的周一日期是否相同来判断
    /// </summary>

    private bool IsOneWeek()
    {
        int toDayOfWeek = (int)_toDay.DayOfWeek;
        toDayOfWeek = toDayOfWeek == 0 ? 7 : toDayOfWeek;// Sunday转7
        int lastDayOfWeek = (int)_lastDay.DayOfWeek;
        lastDayOfWeek = lastDayOfWeek == 0 ? 7 : lastDayOfWeek;
        // 当前时间所在周的周一日期
        DateTime todayOfOne = DateTime.Now.AddDays(1 - toDayOfWeek);
        // 上次签到时间所在周的周一日期
        DateTime lastDayOfOne = DateTime.Now.AddDays(1 - lastDayOfWeek);
        // 比较两个周一日期是否相等
        if (DateTime.Compare(todayOfOne, lastDayOfOne) == 0)
        {
            return true;
        }
        return false;

    }





}
