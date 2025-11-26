using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PrivateRoomManager : MonoBehaviourPunCallbacks
{
    public static PrivateRoomManager Instance;

    private Dictionary<string, string> roomPasswords = new Dictionary<string, string>();
    private string pendingRoomName;
    private string pendingPassword;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreatePrivateRoom(string roomName, string password, RoomOptions roomOptions)
    {
        pendingRoomName = roomName;
        pendingPassword = password;

        // Добавляем пароль в свойства комнаты
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "hasPassword", true },
            { "passwordHash", GetPasswordHash(password) }
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "hasPassword" };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinPrivateRoom(string roomName, string password)
    {
        pendingRoomName = roomName;
        pendingPassword = password;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("hasPassword"))
        {
            string storedHash = (string)PhotonNetwork.CurrentRoom.CustomProperties["passwordHash"];
            string enteredHash = GetPasswordHash(pendingPassword);

            if (storedHash != enteredHash)
            {
                Debug.LogError("Wrong password!");
                PhotonNetwork.LeaveRoom();

                // Показываем сообщение об ошибке
                MenuManager.Instance?.ShowPasswordError();
                return;
            }
        }

        // Пароль верный, продолжаем
        pendingRoomName = null;
        pendingPassword = null;
    }

    private string GetPasswordHash(string password)
    {
        // Простая хэш-функция для демонстрации
        // В реальном проекте используйте более безопасные методы
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
        }
        return sb.ToString();
    }

    // Анти-DDoS: ограничение частоты создания комнат
    private float lastRoomCreationTime;
    private const float ROOM_CREATION_COOLDOWN = 10f;

    public bool CanCreateRoom()
    {
        return Time.time - lastRoomCreationTime >= ROOM_CREATION_COOLDOWN;
    }

    public void RecordRoomCreation()
    {
        lastRoomCreationTime = Time.time;
    }
}