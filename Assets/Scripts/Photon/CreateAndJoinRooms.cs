using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Common;
using Oculus.Platform;
using System;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.IO.Compression;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;
public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    public bool isHost;
    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            CreateNewRoomForLobby("Host");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CreateNewRoomForLobby(string roomToCreate)
    {
        var isValidRoomToJoin = !string.IsNullOrEmpty(roomToCreate);

        if (!isValidRoomToJoin)
        {
            return;
        }

        var roomOptions = new PhotonRealtime.RoomOptions { IsVisible = true, MaxPlayers = 16, EmptyRoomTtl = 0, PlayerTtl = 300000 };

        PhotonNetwork.JoinOrCreateRoom(roomToCreate, roomOptions, PhotonRealtime.TypedLobby.Default);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom("Host");
    }

    public override void OnJoinedRoom()
    {
        if (isHost)
        {
            PhotonNetwork.LoadLevel("Host");
        }
        else
        {
            PhotonNetwork.LoadLevel("SharedSpatialAnchors");
        }
    }
}
