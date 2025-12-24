using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item2 : MonoBehaviour
{
    public Toggle _toggle;
    public Button _button ;

    public void IsParticipated(bool isParticipated)
    {
        _toggle.isOn = isParticipated;
        _button.gameObject.SetActive(!isParticipated);
    }
 

}
