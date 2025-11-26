using UnityEngine;
using UnityEngine.Audio;
using Bash.Framework.Core;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : ManagerBase
{
    protected PoolManager _pool;

    public PoolManager pool
    {
        get
        {
            if(_pool == null)
            {
                _pool = GameRoot.Instance.GetManager<PoolManager>();
            }
            return _pool; 
        }
    }
    protected AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioResource clip, Vector3 pos)
    {
        var go = pool.Spawn("OneShotAudio", pos, Quaternion.identity);
        go.OnSpawned(clip);

        //StartCoroutine(ReturnOnEnd(src, go));
    }

    public void PlaySound(AudioResource clip)
    {
        _audioSource.PlayOneShot(_audioSource.clip);
    }

    public void PlayLoopSound(AudioResource clip)
    {

    }

    //IEnumerator ReturnOnEnd(AudioSource s, PoolingObj go)
    //{
    //    while (s && s.isPlaying) yield return null;
    //    go.GetComponent<ReturnToPool>().Return();
    //}
}
