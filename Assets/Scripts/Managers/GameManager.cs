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
        view.RPC("SetCanPlayNotifier", RpcTarget.All, true);
        view.RPC("UpdateNoticeText", RpcTarget.All, "Game Started");

    }


    public void IncreaseInfectedCount()
    {
        if (++infectionCount >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            infectionCount = 0;
            view.RPC("UpdateNoticeText", RpcTarget.All, "Game Over, all infected!");

            StartCoroutine(DoWithDelay(() =>
            {
                view.RPC("SetCanPlayNotifier", RpcTarget.All, false);
                view.RPC("UpdateNoticeText", RpcTarget.All, "Game Starting");
                // didGameStart = false;
                // view.RPC("SetCanPlayNotifier", RpcTarget.All, true);

            }, 5f));


            // StartCoroutine(DoWithDelay(() =>
            // {
            //     view.RPC("SetCanPlayNotifier", RpcTarget.All, true);
            //     view.RPC("UpdateNoticeText", RpcTarget.All, "Game Started");
            // }, 15f));
        }
    }

    [PunRPC]
    void UpdateNoticeText(string newText)
    {
        CanvasManager.instance.UpdateNoticeText(newText);
    }

    [PunRPC]
    void SetCanPlayNotifier(bool value)
    {
        CanPlayNotifier?.Invoke(value);
    }

    IEnumerator DoWithDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
