using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField] PlayerDetails playerDetails;
    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        if (!playerDetails)
            playerDetails = GetComponent<PlayerDetails>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //View hasn't been set yet
        if (!view) return;

        //Check ownership
        if (!view.IsMine) return;

        //Check if we're already infected
        if (playerDetails.isInfected) return;

        //Check if the player grabbing you is infected
        if (!other.GetComponentInParent<PlayerDetails>().isInfected) return;

        playerDetails.SetInfection(true);

    }
}
