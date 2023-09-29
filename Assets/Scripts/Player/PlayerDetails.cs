using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerDetails : MonoBehaviour
{
    public bool isInfected { private set; get; }
    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }


    // Called either at start or when infected collide with you 
    public void SetInfection()
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        view.RPC("SetInfectionRPC", RpcTarget.All);

        //Not using RPC cause SetInfecedCosmetics calls PhotonVRManager which handles RPC
        GetComponent<SetCosmetics>().SetInfectedCosmetics();

    }

    [PunRPC]
    void SetInfectionRPC()
    {
        isInfected = true;
        if (!PhotonNetwork.IsMasterClient) return;
        GameManager.instance.IncreaseInfectedCount();
    }






}
