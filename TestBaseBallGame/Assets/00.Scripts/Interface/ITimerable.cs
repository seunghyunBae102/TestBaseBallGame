using System;
using UnityEngine;


 public interface ITimerCombo
{
  public void CheckTime(float delta);
public void RunMethod();
  }

  public class TimerCombo : ITimerCombo
{
    public Action method;
    public float remainingTime;

    public TimerCombo(Action a, float remainintT)
    {
        method = a;
        remainingTime = remainintT;
    }

    public void CheckTime(float delta)
    {
        remainingTime -= delta;
        if (remainingTime <= 0)
        {
            RunMethod();
        }
    }
    public void RunMethod()
    {
        method?.Invoke();
    }
}

public class TimerComboOneParam<T> : ITimerCombo
{
    public Action<T> method;
    public float remainingTime;
    public T param;

    public TimerComboOneParam(Action<T> a, float remainintT,T param)
    {
        method = a;
        remainingTime = remainintT;
        this.param = param;
    }

    public void CheckTime(float delta)
    {
        remainingTime -= delta;
        if (remainingTime <= 0)
        {
            RunMethod();
        }
    }
    public void RunMethod()
    {
        method?.Invoke(param);
    }
}

public class TimerComboOneParam<T1,T2> : ITimerCombo
{
    public Action<T1,T2> method;
    public float remainingTime;
    public T1 param;
    public T2 param2;

    public TimerComboOneParam(Action<T1,T2> a, float remainintT, T1 param, T2 param2)
    {
        method = a;
        remainingTime = remainintT;
        this.param = param;
        this.param2 = param2;
    }

    public void CheckTime(float delta)
    {
        remainingTime -= delta;
        if (remainingTime <= 0)
        {
            RunMethod();
        }
    }
    public void RunMethod()
    {
        method?.Invoke(param,param2);
    }
}