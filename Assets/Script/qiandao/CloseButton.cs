using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        if (_btn != null)
        {
            // 绑定点击事件
            _btn.onClick.AddListener(OnCloseClick);
        }
        else
        {
            Debug.LogError("CloseButton.cs 没有挂在 Button 上！");
        }
    }

    private void OnCloseClick()
    {
        // 隐藏按钮所在的父级 UI 面板
        transform.parent.gameObject.SetActive(false);

        // 如果想隐藏更上层对象，可以用：
        // transform.root.gameObject.SetActive(false);
    }
}
