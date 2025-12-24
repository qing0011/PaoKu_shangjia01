using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashToMenu : MonoBehaviour
{

    public GameObject theLogo;

    private void Start()
    {
        StartCoroutine(RunSplash());
    }

    IEnumerator RunSplash()
    {
        yield return new WaitForSeconds(2.5f);
        theLogo.SetActive(true);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);//loadScene是加载场景几。打包在buildingsetting里面的先后顺序



    }



}
