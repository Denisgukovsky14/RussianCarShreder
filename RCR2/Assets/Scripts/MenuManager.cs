using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager Instance;

    [Header("UI References")]
    public TMP_InputField playerNameInput;
    public InputField createInput;
    public InputField joinInput;
    public Text connectionStatusText;
    public Button createButton;
    public Button joinButton;
    public Button retryButton;
<<<<<<< Updated upstream
    public GameObject loadingPanel;
    public Toggle IfOffline;

=======
    public Button searchGameButton;
    public Toggle IfOffline;

    [Header("Private Room Settings")]
    public Toggle privateRoomToggle;
    public TMP_InputField roomPasswordInput;
    public GameObject privateRoomSettings;

    [Header("Room Browser UI")]
    public GameObject roomBrowserPanel;
    public ScrollRect roomScrollView;
    public GameObject roomEntryPrefab;
    public GameObject passwordPromptPanel;
    public InputField passwordInputField;
    public Button passwordSubmitButton;
    public Button passwordCancelButton;

    [Header("Connection Settings")]
    public GameObject loadingPanel;

>>>>>>> Stashed changes
    private bool isConnected = false;
    private bool isInLobby = false;
    private float lastKeepAliveTime = 0f;
    private const float keepAliveInterval = 10f;

    private int connectionRetries = 0;
    private const int maxRetries = 3;

<<<<<<< Updated upstream
    void Start()
    {

=======
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private string selectedRoomName = "";
    private bool selectedRoomHasPassword = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Детальная проверка Photon
        Debug.Log($"=== PHOTON DIAGNOSTICS ===");
        Debug.Log($"AppId: {PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime}");
        Debug.Log($"Server: {PhotonNetwork.PhotonServerSettings.AppSettings.Server}");
        Debug.Log($"Port: {PhotonNetwork.PhotonServerSettings.AppSettings.Port}");
        Debug.Log($"Protocol: {PhotonNetwork.PhotonServerSettings.AppSettings.Protocol}");
        Debug.Log($"Fixed Region: {PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion}");
        Debug.Log($"Network Logging: {PhotonNetwork.LogLevel}");
        Debug.Log($"===========================");

        // Инициализация имени игрока
>>>>>>> Stashed changes
        if (playerNameInput != null && string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.text = "Player_" + Random.Range(1000, 9999);
        }

        IfOffline.isOn = false; 
        Debug.Log("Initializing Photon...");

        // Важные настройки
        Application.runInBackground = true;

        // Оптимальные настройки Photon
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.KeepAliveInBackground = 30;

        // Используем TCP для лучшей стабильности в РФ
        PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ConnectionProtocol.Tcp;
        PhotonNetwork.PhotonServerSettings.AppSettings.EnableProtocolFallback = true;

        // Настройка региона
        SetOptimalRegion();

        // Проверяем настройки Photon
        Debug.Log($"Photon AppId: {PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime}");
        Debug.Log($"Photon Region: {PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion}");
        Debug.Log($"Photon Protocol: {PhotonNetwork.PhotonServerSettings.AppSettings.Protocol}");

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

    private void SetOptimalRegion()
    {
<<<<<<< Updated upstream
        bool allGood = true;
        if (createInput == null) { Debug.LogError("createInput is not assigned!"); allGood = false; }
        if (joinInput == null) { Debug.LogError("joinInput is not assigned!"); allGood = false; }
        if (connectionStatusText == null) { Debug.LogError("connectionStatusText is not assigned!"); allGood = false; }
        if (createButton == null) { Debug.LogError("createButton is not assigned!"); allGood = false; }
        if (joinButton == null) { Debug.LogError("joinButton is not assigned!"); allGood = false; }
        if (retryButton == null) { Debug.LogError("retryButton is not assigned!"); allGood = false; }
        if (loadingPanel == null) { Debug.LogError("loadingPanel is not assigned!"); allGood = false; }
        return allGood;
=======
        if (!string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion))
        {
            return;
        }

        // Для РФ лучше использовать eu (Европу) как fallback
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
>>>>>>> Stashed changes
    }

    private void InitializeUI()
    {
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetGameObjectActive(retryButton?.gameObject, false);
<<<<<<< Updated upstream
        SafeSetGameObjectActive(loadingPanel, true);
=======
        SafeSetGameObjectActive(roomBrowserPanel, false);
        SafeSetGameObjectActive(passwordPromptPanel, false);
        SafeSetGameObjectActive(privateRoomSettings, false);
>>>>>>> Stashed changes

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, true);

        // Подписка на кнопки
        if (retryButton != null)
            retryButton.onClick.AddListener(ManualReconnect);

<<<<<<< Updated upstream
        UpdateConnectionStatus("Initializing...");
    }

=======
        if (searchGameButton != null)
            searchGameButton.onClick.AddListener(ShowRoomBrowser);

        if (passwordSubmitButton != null)
            passwordSubmitButton.onClick.AddListener(JoinRoomWithPassword);

        if (passwordCancelButton != null)
            passwordCancelButton.onClick.AddListener(CancelPasswordJoin);

        if (privateRoomToggle != null)
            privateRoomToggle.onValueChanged.AddListener(OnPrivateRoomToggleChanged);

        UpdateConnectionStatus("Initializing...");
    }

    public void OnPrivateRoomToggleChanged(bool isPrivate)
    {
        SafeSetGameObjectActive(privateRoomSettings, isPrivate);
        if (roomPasswordInput != null && isPrivate)
        {
            roomPasswordInput.text = "";
        }
    }

    // ВАЛИДАТОР НИКНЕЙМА
    public string ValidateAndSanitizeNickname(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
        {
            return "Player_" + Random.Range(1000, 9999);
        }

        // Ограничение длины (3-20 символов)
        if (nickname.Length > 20)
        {
            nickname = nickname.Substring(0, 20);
            Debug.LogWarning("Nickname too long, truncated to 20 characters");
        }

        if (nickname.Length < 3)
        {
            nickname = "Player_" + Random.Range(1000, 9999);
            Debug.LogWarning("Nickname too short, using generated name");
        }

        // Проверка на разрешенные символы
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_\- ]+$");
        if (!regex.IsMatch(nickname))
        {
            Debug.LogWarning("Nickname contains invalid characters, sanitizing...");
            System.Text.RegularExpressions.Regex sanitizeRegex = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z0-9_\- ]");
            nickname = sanitizeRegex.Replace(nickname, "");

            if (string.IsNullOrEmpty(nickname.Trim()))
            {
                nickname = "Player_" + Random.Range(1000, 9999);
            }
        }

        nickname = nickname.Trim();
        while (nickname.Contains("  "))
        {
            nickname = nickname.Replace("  ", " ");
        }

        return nickname;
    }

    public void OnNicknameValueChanged()
    {
        if (playerNameInput != null)
        {
            string validatedNickname = ValidateAndSanitizeNickname(playerNameInput.text);
            if (validatedNickname != playerNameInput.text)
            {
                playerNameInput.text = validatedNickname;
            }
        }
    }

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

        // Автовыбор региона
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = null;

=======
>>>>>>> Stashed changes
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully connected to Master Server in region: " + PhotonNetwork.CloudRegion);
        connectionRetries = 0; // Сбрасываем счетчик ретраев
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
<<<<<<< Updated upstream
        SafeSetGameObjectActive(loadingPanel, false);
        SafeSetGameObjectActive(retryButton?.gameObject, false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected: {cause}");

        if (connectionRetries < maxRetries)
        {
            connectionRetries++;
            UpdateConnectionStatus($"Reconnecting... Attempt {connectionRetries}/{maxRetries}");
            Invoke("ConnectToPhoton", 2f); // Повторная попытка через 2 сек
        }
        else
        {
            UpdateConnectionStatus($"Failed to connect after {maxRetries} attempts");
            // Показать кнопку переподключения
        }

        isConnected = false;
        isInLobby = false;

        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetGameObjectActive(loadingPanel, false);

        UpdateConnectionStatus($"Disconnected: {cause}");
        SafeSetGameObjectActive(retryButton?.gameObject, true);

        try
        {
            GameObject.FindWithTag("Platform").GetComponent<DefaultStatics>().Counter -= 1;
        }
        catch
        {
            Debug.Log(" Не найдено ");
        }
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
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            PhotonNetwork.NickName = playerNameInput.text;
            Debug.Log($" Ник установлен: {PhotonNetwork.NickName}");
=======
        SafeSetButtonInteractable(searchGameButton, true);

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, false);

        SafeSetGameObjectActive(retryButton?.gameObject, false);
    }

    // ОСНОВНОЙ МЕТОД ДЛЯ СОЗДАНИЯ КОМНАТЫ
    public void CreateRoom()
    {
        Debug.Log("=== НАЧАЛО СОЗДАНИЯ КОМНАТЫ ===");

        // Анти-DDoS проверка
        if (PrivateRoomManager.Instance != null && !PrivateRoomManager.Instance.CanCreateRoom())
        {
            UpdateConnectionStatus("Please wait before creating another room");
            return;
        }

        // Валидация никнейма
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            string validatedNickname = ValidateAndSanitizeNickname(playerNameInput.text);
            PhotonNetwork.NickName = validatedNickname;
            playerNameInput.text = validatedNickname;
            Debug.Log($"Никнейм установлен: {PhotonNetwork.NickName}");
>>>>>>> Stashed changes
        }
        else
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
            Debug.Log($" Случайный ник: {PhotonNetwork.NickName}");
        }

        if (IfOffline)
        {
<<<<<<< Updated upstream
            OnDisconnected(new DisconnectCause());
=======
            Debug.Log("Создание офлайн комнаты");
>>>>>>> Stashed changes
            CreateOfflineRoom();
            return;
        }

        if (!isInLobby)
        {
            Debug.LogError("Не в лобби! Нельзя создать комнату");
            UpdateConnectionStatus("Не в лобби! Пожалуйста, подождите...");
            return;
        }

        string roomName = createInput != null ? createInput.text.Trim() : "";
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room_" + Random.Range(1000, 9999);
            if (createInput != null) createInput.text = roomName;
        }

<<<<<<< Updated upstream
        UpdateConnectionStatus("Creating room...");
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetGameObjectActive(loadingPanel, true);

        //  ПРОСТЫЕ НАСТРОЙКИ КОМНАТЫ
=======
        bool isPrivate = privateRoomToggle != null && privateRoomToggle.isOn;
        string password = isPrivate && roomPasswordInput != null ? roomPasswordInput.text.Trim() : "";

        DebugRoomCreation(roomName, isPrivate, password);

        UpdateConnectionStatus("Создание комнаты...");
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetButtonInteractable(searchGameButton, false);

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, true);

>>>>>>> Stashed changes
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 8,
            IsVisible = true,
<<<<<<< Updated upstream
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

=======
            IsOpen = true,
            EmptyRoomTtl = 60000,
            PlayerTtl = 30000,
            CleanupCacheOnLeave = true
        };

        if (isPrivate && !string.IsNullOrEmpty(password))
        {
            Debug.Log("Создание приватной комнаты через PrivateRoomManager");
            if (PrivateRoomManager.Instance != null)
            {
                PrivateRoomManager.Instance.CreatePrivateRoom(roomName, password, roomOptions);
                PrivateRoomManager.Instance.RecordRoomCreation();
            }
            else
            {
                Debug.LogError("PrivateRoomManager.Instance is null!");
                PhotonNetwork.CreateRoom(roomName, roomOptions);
            }
        }
        else
        {
            Debug.Log("Создание публичной комнаты");
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "hasPassword", false }
            };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "hasPassword" };
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    // БРАУЗЕР КОМНАТ
    public void ShowRoomBrowser()
    {
        if (!isInLobby)
        {
            UpdateConnectionStatus("Not in lobby! Please wait...");
            return;
        }

        SafeSetGameObjectActive(roomBrowserPanel, true);
        UpdateRoomList();
    }

    public void HideRoomBrowser()
    {
        SafeSetGameObjectActive(roomBrowserPanel, false);
    }

    private void UpdateRoomList()
    {
        // Очищаем существующий список
        if (roomScrollView != null && roomScrollView.content != null)
        {
            foreach (Transform child in roomScrollView.content.transform)
            {
                Destroy(child.gameObject);
            }

            // Создаем элементы для каждой комнаты
            foreach (var roomInfo in cachedRoomList.Values)
            {
                if (roomEntryPrefab != null)
                {
                    GameObject roomEntry = Instantiate(roomEntryPrefab, roomScrollView.content.transform);
                    RoomEntryUI entryUI = roomEntry.GetComponent<RoomEntryUI>();

                    if (entryUI != null)
                    {
                        bool hasPassword = roomInfo.CustomProperties.ContainsKey("hasPassword") &&
                                          (bool)roomInfo.CustomProperties["hasPassword"];

                        entryUI.Setup(roomInfo.Name, roomInfo.PlayerCount, roomInfo.MaxPlayers, hasPassword);
                    }
                }
            }
        }
    }

    // Публичный метод для выбора комнаты (вызывается из RoomEntryUI)
    public void SelectRoom(string roomName, bool hasPassword)
    {
        selectedRoomName = roomName;
        selectedRoomHasPassword = hasPassword;

        if (hasPassword)
        {
            ShowPasswordPrompt();
        }
        else
        {
            JoinSelectedRoom("");
        }
    }

    private void ShowPasswordPrompt()
    {
        SafeSetGameObjectActive(passwordPromptPanel, true);
        if (passwordInputField != null)
        {
            passwordInputField.text = "";
        }
    }

    private void HidePasswordPrompt()
    {
        SafeSetGameObjectActive(passwordPromptPanel, false);
    }

    public void JoinRoomWithPassword()
    {
        string password = passwordInputField != null ? passwordInputField.text.Trim() : "";
        JoinSelectedRoom(password);
        HidePasswordPrompt();
    }

    private void CancelPasswordJoin()
    {
        HidePasswordPrompt();
        selectedRoomName = "";
    }

    private void JoinSelectedRoom(string password)
    {
        // Валидация никнейма
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            string validatedNickname = ValidateAndSanitizeNickname(playerNameInput.text);
            PhotonNetwork.NickName = validatedNickname;
            playerNameInput.text = validatedNickname;
            Debug.Log($"Никнейм установлен: {PhotonNetwork.NickName}");
        }
        else
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
        }

        if (!isInLobby)
        {
            UpdateConnectionStatus("Not in lobby! Please wait...");
            return;
        }

        UpdateConnectionStatus("Joining room...");
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetButtonInteractable(searchGameButton, false);

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, true);

        if (selectedRoomHasPassword && !string.IsNullOrEmpty(password))
        {
            // Присоединение к приватной комнате через PrivateRoomManager
            if (PrivateRoomManager.Instance != null)
            {
                PrivateRoomManager.Instance.JoinPrivateRoom(selectedRoomName, password);
            }
            else
            {
                PhotonNetwork.JoinRoom(selectedRoomName);
            }
        }
        else
        {
            // Присоединение к публичной комнате
            PhotonNetwork.JoinRoom(selectedRoomName);
        }
    }

    // МЕТОД ДЛЯ ПРИСОЕДИНЕНИЯ ПО ИМЕНИ (старый функционал)
>>>>>>> Stashed changes
    public void JoinRoom()
    {
        // Убеждаемся, что никнейм установлен до присоединения
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            string validatedNickname = ValidateAndSanitizeNickname(playerNameInput.text);
            PhotonNetwork.NickName = validatedNickname;
            Debug.Log($"Никнейм установлен: {PhotonNetwork.NickName}");
        }
        else
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
        }

        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            PhotonNetwork.NickName = playerNameInput.text;
            Debug.Log($" Ник установлен: {PhotonNetwork.NickName}");
        }
        else
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
            Debug.Log($" Случайный ник: {PhotonNetwork.NickName}");
        }

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
<<<<<<< Updated upstream
        SafeSetGameObjectActive(loadingPanel, true);
=======
        SafeSetButtonInteractable(searchGameButton, false);

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, true);
>>>>>>> Stashed changes

        PhotonNetwork.JoinRoom(roomName);
    }

<<<<<<< Updated upstream
    private string ValidateAndSanitizeNickname(string nickname)
=======
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
>>>>>>> Stashed changes
    {
        if (string.IsNullOrEmpty(nickname))
            return "Player_" + Random.Range(1000, 9999);

<<<<<<< Updated upstream
        // Ограничение длины
        if (nickname.Length > 20)
            nickname = nickname.Substring(0, 20);

        // Удаление опасных символов
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z0-9_\- ]");
        nickname = regex.Replace(nickname, "");

        return nickname.Trim();
=======
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                cachedRoomList.Remove(roomInfo.Name);
            }
            else
            {
                cachedRoomList[roomInfo.Name] = roomInfo;
            }
        }

        if (roomBrowserPanel != null && roomBrowserPanel.activeInHierarchy)
        {
            UpdateRoomList();
        }
    }

    public void ShowPasswordError()
    {
        UpdateConnectionStatus("Wrong password! Please try again.");
        ShowPasswordPrompt();
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
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected: {cause}");

        if (connectionRetries < maxRetries)
        {
            connectionRetries++;
            UpdateConnectionStatus($"Reconnecting... Attempt {connectionRetries}/{maxRetries}");
            Invoke("ConnectToPhoton", 2f);
        }
        else
        {
            UpdateConnectionStatus($"Failed to connect after {maxRetries} attempts");
        }

        isConnected = false;
        isInLobby = false;

        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetButtonInteractable(searchGameButton, false);

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, false);

        UpdateConnectionStatus($"Disconnected: {cause}");
        SafeSetGameObjectActive(retryButton?.gameObject, true);

        try
        {
            GameObject.FindWithTag("Platform").GetComponent<DefaultStatics>().Counter -= 1;
        }
        catch
        {
            Debug.Log("Не найдено");
        }
>>>>>>> Stashed changes
    }

    public void ManualReconnect()
    {
        UpdateConnectionStatus("Reconnecting...");
        SafeSetGameObjectActive(retryButton?.gameObject, false);
<<<<<<< Updated upstream
        SafeSetGameObjectActive(loadingPanel, true);
=======

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, true);

>>>>>>> Stashed changes
        ConnectToPhoton();
    }

    public void OnConnectedToServer( DisconnectCause cause ) 
    {
        Debug.Log(cause);
    }

    public override void OnCreatedRoom()
    {
<<<<<<< Updated upstream
        Debug.Log(" Room created successfully!");
        UpdateConnectionStatus("Room created! Waiting for players...");

        //  УБИРАЕМ КОРУТИНУ - просто ждем игроков
        // Автоматически перейдем в игру когда будет достаточно игроков
        // или через какое-то время
=======
        Debug.Log("Комната успешно создана!");
        Debug.Log($"Название комнаты: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Игроков в комнате: {PhotonNetwork.CurrentRoom.PlayerCount}");
        UpdateConnectionStatus("Комната создана! Ожидание игроков...");
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
            // Если есть 2+ игрока, начинаем сразу
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient)
=======
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 1 && PhotonNetwork.IsMasterClient)
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        Debug.LogError($" Room creation failed: {message}");
        UpdateConnectionStatus($"Create failed: {message}");
=======
        Debug.LogError($"Ошибка создания комнаты: {message} (код: {returnCode})");
        UpdateConnectionStatus($"Ошибка создания: {message}");
>>>>>>> Stashed changes

        // Восстанавливаем кнопки
        SafeSetButtonInteractable(createButton, true);
        SafeSetButtonInteractable(joinButton, true);
<<<<<<< Updated upstream
        SafeSetGameObjectActive(loadingPanel, false);
=======
        SafeSetButtonInteractable(searchGameButton, true);

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, false);
>>>>>>> Stashed changes
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($" Join room failed: {message}");
        UpdateConnectionStatus($"Join failed: {message}");

        SafeSetButtonInteractable(createButton, true);
        SafeSetButtonInteractable(joinButton, true);
<<<<<<< Updated upstream
        SafeSetGameObjectActive(loadingPanel, false);
=======
        SafeSetButtonInteractable(searchGameButton, true);

        if (loadingPanel != null)
            SafeSetGameObjectActive(loadingPanel, false);
>>>>>>> Stashed changes
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(" Joined room successfully!");
        UpdateConnectionStatus($"Joined room: {PhotonNetwork.CurrentRoom.Name}");

        // Немедленная синхронизация никнейма
        if (PhotonNetwork.InRoom)
        {
            Debug.Log($"Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}");
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                Debug.Log($" - {player.NickName} (Actor: {player.ActorNumber})");
            }
        }

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

<<<<<<< Updated upstream
        // Обновляем отображение списка игроков
        PlayerInfoManager.Instance?.PrintAllPlayers();

        // Если достаточно игроков, начинаем игру
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient)
=======
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 1 && PhotonNetwork.IsMasterClient)
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    //  ОСТОРОЖНО: Убираем проблемные методы если они вызывают краш
    /*
    void OnApplicationFocus(bool hasFocus) { }
    void OnApplicationPause(bool pauseStatus) { }
    */
=======
    private void DebugRoomCreation(string roomName, bool isPrivate, string password)
    {
        Debug.Log($"Создание комнаты: {roomName}");
        Debug.Log($"Приватная: {isPrivate}");
        Debug.Log($"Пароль: {(string.IsNullOrEmpty(password) ? "нет" : "есть")}");
        Debug.Log($"В лобби: {isInLobby}");
        Debug.Log($"Подключен: {isConnected}");
        Debug.Log($"Photon подключен: {PhotonNetwork.IsConnected}");
    }
>>>>>>> Stashed changes
}