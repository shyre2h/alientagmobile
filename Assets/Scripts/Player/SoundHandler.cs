using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GorillaLocomotion;
using Unity.VisualScripting;

public class SoundHandler : MonoBehaviour
{

    [SerializeField] Player playerScript;
    bool canPlaySound = false;

    private void OnEnable()
    {
        Blink.instance.BlinkNotifier += BlinkListener;
    }


    private void OnDisable()
    {
        Blink.instance.BlinkNotifier -= BlinkListener;
    }



    void BlinkListener(bool value)
    {
        canPlaySound = !value;
    }




    private void OnTriggerEnter(Collider other)
    {
        if (!canPlaySound) return;

        if (other.tag == "Rock")
            playerScript.HitObject(0);

        else if (other.tag == "Brick")
            playerScript.HitObject(1);
    }

}

