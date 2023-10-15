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
    public void SetInfection(bool value)
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        if (value)
        {
            view.RPC("SetInfectionRPC", RpcTarget.All);

            //Not using RPC cause SetInfecedCosmetics calls PhotonVRManager which handles RPC
            GetComponent<SetCosmetics>().SetInfectedCosmetics();
        }
        else
        {
            view.RPC("SetNotInfectionRPC", RpcTarget.All);
            GetComponent<SetCosmetics>().SetNotInfectedCosmetics();
        }
    }

    [PunRPC]
    void SetInfectionRPC()
    {
        isInfected = true;
        if (!PhotonNetwork.IsMasterClient) return;
        GameManager.instance.IncreaseInfectedCount();
    }



    [PunRPC]
    void SetNotInfectionRPC()
    {
        isInfected = false;
    }




}
