using UnityEngine;
using Bash.Framework.Core;
[RequireComponent(typeof(AudioSource))]
public class SoundPoolObj : PoolingObj
{
    protected AudioSource _audioSource;


    public override void OnSpawned(params object[] objs)
    {
        base.OnSpawned(objs);
        if (objs.Length > 0 && objs[0] is AudioClip clip)
        {
            if(_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            if (_audioSource != null)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }
        
            GameRoot.Instance.GetManager<TimerManager>().AddTimer(new TimerCombo(TimeToDie,clip.length / _audioSource.pitch));
        }

        
    }

    public void TimeToDie() 
    {
        OnDespawned();
    }
}
