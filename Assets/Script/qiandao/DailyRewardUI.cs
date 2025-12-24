using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DailyRewardUI : MonoBehaviour
{
    [Header("UI组件")]
    public GameObject panel; // 签到面板
    public Button signButton; // 签到按钮
    public TMP_Text signButtonText;
    public TMP_Text consecutiveDaysText;
    public Transform rewardsContainer; // 奖励项容器
    public GameObject rewardItemPrefab; // 单个奖励项预制体
// 在DailyRewardUI中添加
    public Button closeButton;

    private void Start()
    {
        signButton.onClick.AddListener(OnSignButtonClick);
        UpdateUI();
       
    }

    // 打开/关闭签到面板
    public void TogglePanel(bool show)
    {
        panel.SetActive(show);
        if (show) UpdateUI();
    }

    public void UpdateUI()
    {
        var (consecutiveDays, signedToday, lastSignDate) = DailyRewardManager.Instance.GetSignStatus();

        // 更新连续签到天数显示
        consecutiveDaysText.text = $"连续签到: {consecutiveDays}天";

        // 更新签到按钮状态
        signButton.interactable = !signedToday;
        signButtonText.text = signedToday ? "今日已签到" : "签到";

        // 清空现有奖励项
        foreach (Transform child in rewardsContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建奖励项
        foreach (var reward in DailyRewardManager.Instance.rewards)
        {
            var item = Instantiate(rewardItemPrefab, rewardsContainer);
            var rewardUI = item.GetComponent<DailyRewardItemUI>();

            bool isReceived = reward.day <= consecutiveDays;
            bool isToday = reward.day == consecutiveDays + 1;

            rewardUI.Setup(reward, isReceived, isToday);
        }
    }

    private void OnSignButtonClick()
    {
        var reward = DailyRewardManager.Instance.SignToday();
        if (reward != null)
        {
            // 显示获得奖励效果
            ShowRewardEffect(reward);
            UpdateUI();
        }
    }

    private void ShowRewardEffect(DailyRewardManager.Reward reward)
    {
        // 这里可以添加获得奖励的特效
        Debug.Log($"获得奖励: {reward.coins}金币");

        // 示例: 弹出奖励获取提示
       // ToastMessage.Instance.Show($"签到成功! 获得{reward.coins}金币");
    }
}