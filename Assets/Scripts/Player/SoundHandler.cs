using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GorillaLocomotion;
using Unity.VisualScripting;

public class SoundHandler : MonoBehaviour
{

    [SerializeField] Player playerScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Rock")
            playerScript.HitObject(0);

        else if (other.tag == "Brick")
            playerScript.HitObject(1);
    }

}

