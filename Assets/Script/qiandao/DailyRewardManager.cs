using UnityEngine;
using System;
using System.Collections.Generic;

public class DailyRewardManager : MonoBehaviour
{
    public static DailyRewardManager Instance;

    // 签到奖励配置
    [System.Serializable]
    public class Reward
    {
        public int day;
        public int coins;
        public bool isSpecial; // 是否特殊奖励(如第7天)
    }

    [Header("签到奖励配置")]
    public Reward[] rewards; // 在Inspector中配置

    // 玩家数据键名
    private const string LAST_SIGN_DATE_KEY = "LastSignDate";
    private const string CONSECUTIVE_DAYS_KEY = "ConsecutiveDays";
    private const string MONTH_SIGNED_KEY = "MonthSigned";

    private DateTime lastSignDate;
    private int consecutiveDays;
    private HashSet<int> signedDaysThisMonth = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }

    private void LoadData()
    {
        // 加载最后一次签到日期
        string dateStr = PlayerPrefs.GetString(LAST_SIGN_DATE_KEY, "");
        if (!string.IsNullOrEmpty(dateStr))
        {
            lastSignDate = DateTime.Parse(dateStr);
        }

        // 加载连续签到天数
        consecutiveDays = PlayerPrefs.GetInt(CONSECUTIVE_DAYS_KEY, 0);

        // 加载本月已签到日期
        string signedDays = PlayerPrefs.GetString(MONTH_SIGNED_KEY, "");
        if (!string.IsNullOrEmpty(signedDays))
        {
            string[] days = signedDays.Split(',');
            foreach (string day in days)
            {
                if (int.TryParse(day, out int dayNum))
                {
                    signedDaysThisMonth.Add(dayNum);
                }
            }
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetString(LAST_SIGN_DATE_KEY, lastSignDate.ToString());
        PlayerPrefs.SetInt(CONSECUTIVE_DAYS_KEY, consecutiveDays);

        // 保存本月已签到日期
        string signedDaysStr = string.Join(",", signedDaysThisMonth);
        PlayerPrefs.SetString(MONTH_SIGNED_KEY, signedDaysStr);

        PlayerPrefs.Save();
    }

    // 检查是否可以签到
    public bool CanSignToday()
    {
        DateTime today = DateTime.Today;

        // 从未签到过
        if (lastSignDate == default(DateTime))
            return true;

        // 今天已经签到过
        if (lastSignDate.Date == today)
            return false;

        // 检查是否是连续签到
        TimeSpan timeSpan = today - lastSignDate.Date;
        if (timeSpan.TotalDays > 1)
        {
            // 超过1天未签到，重置连续天数
            consecutiveDays = 0;
        }

        return true;
    }

    // 执行签到
    public Reward SignToday()
    {
        if (!CanSignToday())
        {
            Debug.LogWarning("今天已经签到过了！");
            return null;
        }

        DateTime today = DateTime.Today;
        int dayOfMonth = today.Day;

        // 如果是新月份，重置签到数据
        if (lastSignDate.Month != today.Month || lastSignDate.Year != today.Year)
        {
            consecutiveDays = 0;
            signedDaysThisMonth.Clear();
        }

        // 增加连续签到天数
        consecutiveDays++;
        lastSignDate = today;
        signedDaysThisMonth.Add(dayOfMonth);

        // 获取今日奖励
        Reward todayReward = GetTodayReward();

        // 发放奖励
        if (todayReward != null)
        {
            PlayerStats.AddCoins(todayReward.coins);
            Debug.Log($"签到成功！获得{todayReward.coins}金币");
        }

        SaveData();
        return todayReward;
    }

    // 获取今日应得的奖励
    private Reward GetTodayReward()
    {
        // 连续天数不超过奖励配置长度
        int dayIndex = Mathf.Min(consecutiveDays, rewards.Length) - 1;
        if (dayIndex >= 0 && dayIndex < rewards.Length)
        {
            return rewards[dayIndex];
        }
        return null;
    }

    // 获取当前签到状态
    public (int consecutiveDays, bool signedToday, DateTime lastSignDate) GetSignStatus()
    {
        return (consecutiveDays, lastSignDate.Date == DateTime.Today, lastSignDate);
    }

    // 获取本月签到日历
    public Dictionary<int, bool> GetMonthCalendar()
    {
        DateTime today = DateTime.Today;
        Dictionary<int, bool> calendar = new Dictionary<int, bool>();

        for (int i = 1; i <= DateTime.DaysInMonth(today.Year, today.Month); i++)
        {
            calendar[i] = signedDaysThisMonth.Contains(i);
        }

        return calendar;
    }
}