using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private float _Timer = 0f;
    public bool _Running = true;
    private int Min, Sec, MilliSec;
    private string TimeText;
    [SerializeField] private SoldierUIManager _uiManager;

    void Update()
    {
        if (_Running)
        {
            _Timer += Time.deltaTime;
            Min = (int)(_Timer / 60);
            Sec = (int)(_Timer % 60);
            MilliSec = (int)((_Timer % 1) * 100);

            if(Min >= 10)//分
            {
                TimeText = Min.ToString();
            }
            else
            {
                TimeText = "0" + Min.ToString();
            }

            if (Sec >= 10)//秒
            {
                TimeText += ":" + Sec;
            }
            else
            {
                TimeText += ":0" + Sec;
            }

            if (MilliSec >= 10)//ミリ秒
            {
                TimeText +=":" + MilliSec;
            }
            else
            {
                TimeText +=":0" + MilliSec;
            }
            _uiManager.UpdateTimer(TimeText);
        }
    }
}
