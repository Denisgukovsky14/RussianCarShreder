using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    public Text connectionStatusText;

    private bool isReadyToCreateRooms = false;

    void Start()
    {
        Application.runInBackground = true;
        PhotonNetwork.SerializationRate = 15;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.ConnectUsingSettings();
        UpdateConnectionStatus("Connecting to server...");
        isReadyToCreateRooms = false;

        try
        {
            // Способ 2: через рефлексию (для старых версий)
            var field = typeof(AppSettings).GetField("EnableProtocolEncryption");
            if (field != null) field.SetValue(PhotonNetwork.PhotonServerSettings.AppSettings, false);
        }
        catch
        {
            Debug.Log("Cannot disable encryption - need to update Photon");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        UpdateConnectionStatus("Connected! Joining lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby - ready to create rooms!");
        UpdateConnectionStatus("Ready to play!");
        isReadyToCreateRooms = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        Debug.LogError($"Disconnected: {cause}");
        UpdateConnectionStatus($"Disconnected: {cause}");
        isReadyToCreateRooms = false;
    }

    public void CreateRoom()
    {
        if (isReadyToCreateRooms)
        {
            if (string.IsNullOrEmpty(createInput.text))
            {
                Debug.LogError("Room name is empty!");
                return;
            }

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;

            PhotonNetwork.CreateRoom(createInput.text, roomOptions);
            UpdateConnectionStatus("Creating room...");
        }
        else
        {
            Debug.LogError($"Not ready! Current state: {PhotonNetwork.NetworkClientState}");
            UpdateConnectionStatus($"Please wait... {PhotonNetwork.NetworkClientState}");
        }
    }

    public void JoinRoom()
    {
        if (isReadyToCreateRooms)
        {
            if (string.IsNullOrEmpty(joinInput.text))
            {
                Debug.LogError("Room name is empty!");
                return;
            }

            PhotonNetwork.JoinRoom(joinInput.text);
            UpdateConnectionStatus("Joining room...");
        }
        else
        {
            Debug.LogError($"Not ready! Current state: {PhotonNetwork.NetworkClientState}");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully!");
        UpdateConnectionStatus("Room created!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed: {message}");
        UpdateConnectionStatus($"Create failed: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join room failed: {message}");
        UpdateConnectionStatus($"Join failed: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully!");
        PhotonNetwork.LoadLevel("USSMAP");
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left: {otherPlayer.NickName}");
    }

    private void UpdateConnectionStatus(string status)
    {
        if (connectionStatusText != null)
            connectionStatusText.text = status;

        Debug.Log(status);
    }
}