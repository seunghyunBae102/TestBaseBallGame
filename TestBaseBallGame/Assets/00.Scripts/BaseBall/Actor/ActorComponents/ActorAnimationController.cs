using UnityEngine;

public class ActorAnimationController : ActorComponent
{
    protected Animator _animator;
    public Animator Animator => _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName)
    {
        _animator.Play(animationName);
    }

    public void SetAnimationBool(string parameterName, bool value)
    {
        _animator.SetBool(parameterName, value);
    }

    public void SetAnimationTrigger(string parameterName)
    {
        _animator.SetTrigger(parameterName);
    }
    public void SetAnimationFloat(string parameterName, float value)
    {
        _animator.SetFloat(parameterName, value);
    }

    public void SetAnimationInt(string parameterName, int value)
    {
        _animator.SetInteger(parameterName, value);
    }



}
