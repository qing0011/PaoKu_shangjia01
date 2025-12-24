using UnityEngine;
using System;

public static class SignInManager
{
    private const string LastSignInKey = "LastSignInDate";

    public static void TrySignIn()
    {
        string lastDate = PlayerPrefs.GetString(LastSignInKey, "");
        string today = DateTime.Now.ToString("yyyyMMdd");

        if (lastDate != today)
        {
            PlayerPrefs.SetString(LastSignInKey, today);
            PlayerStats.AddCoins(1); // 奖励 100 金币
            Debug.Log("签到成功，获得100金币！");
        }
        else
        {
            Debug.Log("今日已签到！");
        }
    }
}
