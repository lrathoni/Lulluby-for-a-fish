using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesFollowPath : MonoBehaviour
{
    public float time;
    public float particleSystemIndex;
    public string pathName;

    void Start()
    {
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        // time offset so that particles don't all leave at the same time
        yield return new WaitForSeconds(particleSystemIndex/2);
        // move particles along path (RiverPath0.... RiverPathN)
        iTween.MoveTo( gameObject, iTween.Hash("path", iTweenPath.GetPath(pathName), "easetype", iTween.EaseType.linear, "time", time, "loopType", "loop") );
    }
}
