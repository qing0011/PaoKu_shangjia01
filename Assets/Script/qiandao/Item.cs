using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Toggle _toggle;
    public Button _button ;

    public void IsParticipated(bool isParticipated)
    {
        _toggle.isOn = isParticipated;
        _button.gameObject.SetActive(!isParticipated);
    }
    /// <summary>
    /// 设置按钮是否可点击
    /// </summary>
    public void SetInteractable(bool canClick)
    {
        if (_button != null)
            _button.interactable = canClick;
    }

}
