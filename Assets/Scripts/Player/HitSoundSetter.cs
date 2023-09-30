using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GorillaLocomotion;

public class HitSoundSetter : MonoBehaviour
{
    Player playerScript;
    int hitSound;

    public enum SurfaceType
    {

        Rock,
        Brick,
        Grass,
        Wood,
        Sand
    }

    public SurfaceType surfaceType;



    void Start()
    {
        playerScript = FindObjectOfType<Player>();
        hitSound = (int)surfaceType;
    }


    public void OnCollisionEnter(Collision other)
    {
        playerScript.HitObject(hitSound);
    }
}
