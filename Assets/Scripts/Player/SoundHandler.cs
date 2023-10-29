using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GorillaLocomotion;
using Unity.VisualScripting;

public class SoundHandler : MonoBehaviour
{

    [SerializeField] Player playerScript;
    bool playSound = false;

    private void OnEnable()
    {
        Blink.BlinkNotifier += BlinkListener;
    }


    private void OnDisable()
    {
        Blink.BlinkNotifier -= BlinkListener;
    }



    void BlinkListener(bool value)
    {
        playSound = value;
    }




    private void OnTriggerEnter(Collider other)
    {
        if (!playSound) return;

        if (other.tag == "Rock")
            playerScript.HitObject(0);

        else if (other.tag == "Brick")
            playerScript.HitObject(1);
    }

}

