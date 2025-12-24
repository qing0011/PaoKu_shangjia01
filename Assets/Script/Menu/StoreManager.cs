using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    private static StoreManager Instance;

    public TMP_Text coinText;
    public TMP_Text itemCountText;

    private static int itemCount = 0;
    public Button closeButton;
    [Header("面板对象")]
    public GameObject storePanel; // 你要关闭的设置弹窗对象

    void Awake() => Instance = this;
    private void Start()
    {
        UpdateUI();
        // 关闭按钮绑定
        closeButton.onClick.AddListener(CloseSettings);
    }
 
    public void BuyItem()
    {
        int price = 200;
        if (PlayerStats.CurrentCoins >= price)
        {
            PlayerStats.SpendCoins(price);
            itemCount++;
            Debug.Log("购买成功！");
            UpdateUI();
        }
        else
        {
            Debug.Log("金币不足！");
        }
    }

    public static void UpdateUI()
    {
        if (Instance != null)
        {
            Instance.coinText.text = "金币：" + PlayerStats.CurrentCoins;
            //Instance.itemCountText.text = "道具：" + itemCount;
        }
    }

  
    private void CloseSettings()
    {

        storePanel.SetActive(false); //  隐藏设置面板
        AudioManager.PlayClick(); // 播放关闭音效（可选）
    }
}
