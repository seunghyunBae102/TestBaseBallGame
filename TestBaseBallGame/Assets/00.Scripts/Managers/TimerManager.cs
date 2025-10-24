using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : GetableManager
{
    private List<ITimerCombo> _timerComobs = new();

    public void AddTimer(Action a, float time)
    {
        _timerComobs.Add(new TimerCombo(a, time));
    }
    public void AddTimer(ITimerCombo timerCombo)
    {
        _timerComobs.Add(timerCombo);
    }

    private void Update()
    {
        foreach (var item in _timerComobs)
        {
            item.CheckTime(Time.deltaTime);
        }
    }

}
