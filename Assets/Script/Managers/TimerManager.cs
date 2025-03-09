using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager
{
    public static TimerManager Instance;
    public static TimerManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new TimerManager();
        }

        return Instance;
    }

    private float _mf_Timer;

    public void SetTime(float _time)
    {
        _mf_Timer = _time;
    }

    public void UpdateTime(float _time)
    {
        _mf_Timer = Mathf.Clamp(_mf_Timer -= _time, 0, float.MaxValue);
    }

    public float GetTime()=> _mf_Timer;

    public bool IsComplateTime()
    {
        if (_mf_Timer <= 0f)
            return true;

        return false;
    }
}