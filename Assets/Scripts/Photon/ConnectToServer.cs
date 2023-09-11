using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public bool isHost;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");

        if (isHost)
        {
            SceneManager.LoadScene("LobbyHost");
        }
        else
        {

            SceneManager.LoadScene("LobbyClient");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
    }
}
