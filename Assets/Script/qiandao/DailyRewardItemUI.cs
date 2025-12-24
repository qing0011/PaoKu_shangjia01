using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DailyRewardItemUI : MonoBehaviour
{
    [Header("UI组件")]
    public Image icon;
    public TMP_Text dayText;
    public TMP_Text amountText;
    public GameObject checkmark;
    public GameObject highlight;
    public GameObject specialTag;
 
    public void Setup(DailyRewardManager.Reward reward, bool isReceived, bool isToday)
    {
        dayText.text = $"第{reward.day}天";
        amountText.text = $"{reward.coins}金币";

        checkmark.SetActive(isReceived);
        highlight.SetActive(isToday);
        specialTag.SetActive(reward.isSpecial);

        // 根据状态设置颜色
        if (isReceived)
        {
            GetComponent<Image>().color = Color.gray;
        }
        else if (isToday)
        {
            GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }
    }
}