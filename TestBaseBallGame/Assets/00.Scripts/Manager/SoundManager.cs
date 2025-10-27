using UnityEngine;
using UnityEngine.Audio;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : GetableManager
{
    protected PoolManager _pool;
    protected AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetCompoParent(GameManager p)
    {
        _pool = p.GetCompo<PoolManager>();
    }

    public void PlaySound(AudioResource clip, Vector3 pos)
    {
        var go = _pool.Spawn("OneShotAudio", pos, Quaternion.identity);
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