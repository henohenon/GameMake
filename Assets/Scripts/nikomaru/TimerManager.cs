using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private float _Timer = 0f;
    private bool _Running = true;
    private int Min, Sec, MilliSec;
    //[SerializeField] private SoldierUIManager time;

    void Update()
    {
        if (_Running)
        {
            _Timer += Time.deltaTime;
            Min = (int)(_Timer / 60);
            Sec = (int)(_Timer % 60);
            MilliSec = (int)((_Timer % 1) * 100);
            Debug.LogFormat("{0}:{1}:{2}", Min, Sec, MilliSec);

        }
    }

    public void OnStart()
    {
        _Running = true;
    }

    public void OnStop()
    {
        _Running = false;
    }
}
