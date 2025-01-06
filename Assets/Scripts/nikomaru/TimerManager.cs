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
            if(Sec >= 10)
            {
                TimeText = Min + ":" + Sec + ":" + MilliSec;
            } else
            {
                TimeText = Min + ":0" + Sec + ":" + MilliSec;
            }
            _uiManager.UpdateTimer(TimeText);
        }
    }
}
