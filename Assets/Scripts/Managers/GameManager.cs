using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    PhotonView view;
    //No need to update for clients, only master needs it for the update function
    bool didGameStart = false;
    public static Action<bool> CanPlayNotifier;
    int infectionCount;


    void Start()
    {
        instance = this;
        view = GetComponent<PhotonView>();
        AudioManager.instance.PlayBackGroundTrack(true);

    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount < 2) return;
        if (!didGameStart)
            StartGame();


    }


    void StartGame()
    {
        didGameStart = true;
        // TODO: Set a random player as infected
        // var randomPlayer = PhotonNetwork.PlayerList[UnityEngine.Random.Range(0, PhotonNetwork.CountOfPlayersInRooms)];
        CanPlayNotifier?.Invoke(true);
        view.RPC("UpdateNoticeText", RpcTarget.All, "Game Started");

    }


    public void IncreaseInfectedCount()
    {
        if (++infectionCount >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            view.RPC("UpdateNoticeText", RpcTarget.All, "Game Over, all infected!");
        }
    }

    [PunRPC]
    void UpdateNoticeText(string newText)
    {
        CanvasManager.instance.UpdateNoticeText(newText);
    }
}
