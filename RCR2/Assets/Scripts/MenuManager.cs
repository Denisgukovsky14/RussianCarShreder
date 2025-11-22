using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public InputField createInput;
    public InputField joinInput;
    public Text connectionStatusText;
    public Button createButton;
    public Button joinButton;
    public Button retryButton;
    public GameObject loadingPanel;
    public Toggle IfOffline;

    private bool isConnected = false;
    private bool isInLobby = false;
    private float lastKeepAliveTime = 0f;
    private const float keepAliveInterval = 10f;

    void Start()
    {
        IfOffline.isOn = false; 
        Debug.Log("Initializing Photon...");

        // Важные настройки
        Application.runInBackground = true;

        // Настройки Photon
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
        PhotonNetwork.KeepAliveInBackground = 60;

        InitializeUI();
        ConnectToPhoton(); // ТОЛЬКО ОДИН РАЗ!
    }

    void Update()
    {
        //  УПРОЩЕННЫЙ KEEP-ALIVE - без корутин
        if (PhotonNetwork.IsConnected && Time.time - lastKeepAliveTime > keepAliveInterval)
        {
            lastKeepAliveTime = Time.time;
            // Просто обновляем время - без сложных операций
        }
    }

    private bool CheckUIReferences()
    {
        bool allGood = true;
        if (createInput == null) { Debug.LogError("createInput is not assigned!"); allGood = false; }
        if (joinInput == null) { Debug.LogError("joinInput is not assigned!"); allGood = false; }
        if (connectionStatusText == null) { Debug.LogError("connectionStatusText is not assigned!"); allGood = false; }
        if (createButton == null) { Debug.LogError("createButton is not assigned!"); allGood = false; }
        if (joinButton == null) { Debug.LogError("joinButton is not assigned!"); allGood = false; }
        if (retryButton == null) { Debug.LogError("retryButton is not assigned!"); allGood = false; }
        if (loadingPanel == null) { Debug.LogError("loadingPanel is not assigned!"); allGood = false; }
        return allGood;
    }

    private void InitializeUI()
    {
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetGameObjectActive(retryButton?.gameObject, false);
        SafeSetGameObjectActive(loadingPanel, true);

        if (retryButton != null)
            retryButton.onClick.AddListener(ManualReconnect);

        UpdateConnectionStatus("Initializing...");
    }

    private void SafeSetButtonInteractable(Button button, bool interactable)
    {
        if (button != null) button.interactable = interactable;
    }

    private void SafeSetGameObjectActive(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }

    public void ConnectToPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
            return;
        }

        UpdateConnectionStatus("Connecting to Photon...");

        // Автовыбор региона
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = null;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(" Connected to Master Server");
        isConnected = true;
        UpdateConnectionStatus("Connected! Joining lobby...");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(" Joined Lobby");
        isInLobby = true;
        UpdateConnectionStatus("Ready to create or join rooms!");

        SafeSetButtonInteractable(createButton, true);
        SafeSetButtonInteractable(joinButton, true);
        SafeSetGameObjectActive(loadingPanel, false);
        SafeSetGameObjectActive(retryButton?.gameObject, false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        try
        {
            GameObject.FindWithTag("Platform").GetComponent<DefaultStatics>().Counter -= 1;
        }
        catch
        {
            Debug.Log(" Не найдено ");
        }

        Debug.Log($" Disconnected: {cause}");
        isConnected = false;
        isInLobby = false;

        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetGameObjectActive(loadingPanel, false);

        UpdateConnectionStatus($"Disconnected: {cause}");
        SafeSetGameObjectActive(retryButton?.gameObject, true);
    }

    private void CreateOfflineRoom()
    {
        string roomName = createInput != null ? createInput.text.Trim() : "";
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "OfflineRoom_" + Random.Range(1000, 9999);
            if (createInput != null) createInput.text = roomName;
        }

        UpdateConnectionStatus("Creating offline room...");
        PhotonNetwork.CreateRoom(roomName);

        AnalyticsManager.Instance.TrackLevelEvent(2, "start");
    }

    public void CreateRoom()
    {
        if (IfOffline)
        {
            OnDisconnected(new DisconnectCause());
            CreateOfflineRoom();
            return;
        }

        if (!isInLobby)
        {
            UpdateConnectionStatus("Not in lobby! Please wait...");
            return;
        }

        string roomName = createInput != null ? createInput.text.Trim() : "";
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room_" + Random.Range(1000, 9999);
            if (createInput != null) createInput.text = roomName;
        }

        UpdateConnectionStatus("Creating room...");
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetGameObjectActive(loadingPanel, true);

        //  ПРОСТЫЕ НАСТРОЙКИ КОМНАТЫ
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom()
    {
        if (IfOffline) { 
        }

        if (!isInLobby)
        {
            UpdateConnectionStatus("Not in lobby! Please wait...");
            return;
        }

        string roomName = joinInput != null ? joinInput.text.Trim() : "";
        if (string.IsNullOrEmpty(roomName))
        {
            UpdateConnectionStatus("Please enter room name!");
            return;
        }

        UpdateConnectionStatus("Joining room...");
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetGameObjectActive(loadingPanel, true);

        PhotonNetwork.JoinRoom(roomName);
    }

    public void ManualReconnect()
    {
        UpdateConnectionStatus("Reconnecting...");
        SafeSetGameObjectActive(retryButton?.gameObject, false);
        SafeSetGameObjectActive(loadingPanel, true);
        ConnectToPhoton();
    }

    public void OnConnectedToServer( DisconnectCause cause ) 
    {
        Debug.Log(cause);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(" Room created successfully!");
        UpdateConnectionStatus("Room created! Waiting for players...");

        //  УБИРАЕМ КОРУТИНУ - просто ждем игроков
        // Автоматически перейдем в игру когда будет достаточно игроков
        // или через какое-то время
        StartCoroutine(WaitForPlayersOrStart());
    }

    private IEnumerator WaitForPlayersOrStart()
    {
        // Ждем максимум 30 секунд перед стартом
        float waitTime = 30f;
        float elapsed = 0f;

        while (elapsed < waitTime && PhotonNetwork.InRoom)
        {
            UpdateConnectionStatus($"Waiting for players... ({Mathf.RoundToInt(waitTime - elapsed)}s)");

            // Если есть 2+ игрока, начинаем сразу
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient)
            {
                StartGame();
                yield break;
            }

            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        // Если время вышло, начинаем с текущим количеством игроков
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        //AnalyticsManager.Instance.TrackLevelEvent(2, "start");
        UpdateConnectionStatus("Starting game...");
        PhotonNetwork.LoadLevel("GAME");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($" Room creation failed: {message}");
        UpdateConnectionStatus($"Create failed: {message}");

        SafeSetButtonInteractable(createButton, true);
        SafeSetButtonInteractable(joinButton, true);
        SafeSetGameObjectActive(loadingPanel, false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($" Join room failed: {message}");
        UpdateConnectionStatus($"Join failed: {message}");

        SafeSetButtonInteractable(createButton, true);
        SafeSetButtonInteractable(joinButton, true);
        SafeSetGameObjectActive(loadingPanel, false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(" Joined room successfully!");
        UpdateConnectionStatus($"Joined room: {PhotonNetwork.CurrentRoom.Name}");

        //  ПРОСТАЯ ЗАГРУЗКА СЦЕНЫ
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameWithDelay());
        }
    }

    private IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(3f); // Короткая задержка
        PhotonNetwork.LoadLevel("GAME");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined the room");
        UpdateConnectionStatus($"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");

        // Если достаточно игроков, начинаем игру
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    private void UpdateConnectionStatus(string status)
    {
        if (connectionStatusText != null)
            connectionStatusText.text = status;

        Debug.Log($"[MenuManager] {status}");
    }

    //  ОСТОРОЖНО: Убираем проблемные методы если они вызывают краш
    /*
    void OnApplicationFocus(bool hasFocus) { }
    void OnApplicationPause(bool pauseStatus) { }
    */
}