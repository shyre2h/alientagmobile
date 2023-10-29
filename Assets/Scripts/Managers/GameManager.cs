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
    public static Action<bool> SetInfectedNotifier;
    public int infectedActorNumber { private set; get; }
    int infectionCount;

    void Start()
    {
        instance = this;
        view = GetComponent<PhotonView>();
        AudioManager.instance.PlayBackGroundTrack(true);

    }

    void Update()
    {

        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2) return;
        if (!didGameStart)
            StartGame();


    }
    // void OnPhotonPlayerConnected()
    // {
    //     if (!playerList.Contains(PhotonNetwork.playerName))
    //     {
    //         playerList.Add(PhotonNetwork.playerName);
    //     }
    // }


    void StartGame()
    {
        didGameStart = true;

        Player[] players = PhotonNetwork.PlayerList;
        // Get a random player from the list
        Player randomPlayer = players[UnityEngine.Random.Range(0, players.Length)];
        infectedActorNumber = randomPlayer.ActorNumber;

        view.RPC("SetInfected", RpcTarget.All, true, infectedActorNumber);
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
                view.RPC("SetInfected", RpcTarget.All, false, -1);
                view.RPC("UpdateNoticeText", RpcTarget.All, "Game Starting");
            }, 5f));

            StartCoroutine(DoWithDelay(() =>
            {
                didGameStart = false;
            }, 10f));

        }
    }

    [PunRPC]
    void UpdateNoticeText(string newText)
    {
        CanvasManager.instance.UpdateNoticeText(newText);
    }

    [PunRPC]
    void SetInfected(bool value, int infectedActorNumber)
    {
        this.infectedActorNumber = infectedActorNumber;
        SetInfectedNotifier?.Invoke(value);
    }

    IEnumerator DoWithDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
