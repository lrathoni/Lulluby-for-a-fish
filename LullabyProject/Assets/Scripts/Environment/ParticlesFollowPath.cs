using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesFollowPath : MonoBehaviour
{
    public float time;
    public float particleSystemIndex;
    public string pathName;
    //public Transform[] rotationCheckpoints;
    private float yRotation;

    void Start()
    {
        yRotation = -90;
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        // time offset so that particles don't all leave at the same time
        yield return new WaitForSeconds(particleSystemIndex * 0.8f);
        // move particles along path (RiverPath0.... RiverPathN)
        iTween.MoveTo( gameObject, iTween.Hash("path", iTweenPath.GetPath(pathName), "easetype", iTween.EaseType.linear, "time", time, "loopType", "loop") );
        //ContinueRotation();
    }

    void ContinueRotation() 
    {
        /*yRotation = 90;
        float closestPoint;
        float magnitude = 0;
        // work out which checkpoint is the closest to know how to rotate
        foreach (checkpoint in rotationCheckpoints)
        {
            float newMagnitude = gameObject.transform - checkpoint).sqrMagnitude;
            if ( newMagnitude < magnitude )
            {
                magnitude = newMagnitude;
                closestPoint = 
            }
        }*/
                // rotate particles (not single ones, the whole shape)
        // rotate once here, and then continue in the method below
        //iTween.RotateAdd(gameObject, iTween.Hash("y", yRotation, "time", 15, "oncomplete", "ContinueRotation"));
        iTween.RotateAdd(gameObject, iTween.Hash("y", -90, "time", 10, "delay", 5));
        iTween.RotateAdd(gameObject, iTween.Hash("y", 15, "time", 10, "delay", 12));
        iTween.RotateAdd(gameObject, iTween.Hash("y", 90, "time", 10, "delay", 15));
        iTween.RotateAdd(gameObject, iTween.Hash("y", 90, "time", 5, "delay", 18));
        iTween.RotateAdd(gameObject, iTween.Hash("y", -90, "time", 5, "delay", 20));
        iTween.RotateAdd(gameObject, iTween.Hash("y", 90, "time", 5, "delay", 22));
        iTween.RotateAdd(gameObject, iTween.Hash("y", 60, "time", 5, "delay", 26));
        iTween.RotateAdd(gameObject, iTween.Hash("y", -30, "time", 5, "delay", 29));
        iTween.RotateAdd(gameObject, iTween.Hash("y", -30, "time", 5, "delay", 34));
        iTween.RotateAdd(gameObject, iTween.Hash("y", 90, "time", 2, "delay", 35));
    }
}
