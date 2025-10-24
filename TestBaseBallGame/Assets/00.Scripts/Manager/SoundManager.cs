using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class SoundManager : GetableManager
{
    protected PoolManager _pool;
    public void SetCompoParent(GameManager p)
    {
        _pool = p.GetCompo<PoolManager>();
    }

    public void PlayOneShotAt(AudioClip clip, Vector3 pos)
    {
        var go = _pool.Spawn("OneShotAudio", pos, Quaternion.identity);
        var src = go.GetComponent<AudioSource>();
        src.clip = clip;
        src.Play();
        
        //StartCoroutine(ReturnOnEnd(src, go));
    }

    //IEnumerator ReturnOnEnd(AudioSource s, PoolingObj go)
    //{
    //    while (s && s.isPlaying) yield return null;
    //    go.GetComponent<ReturnToPool>().Return();
    //}
}