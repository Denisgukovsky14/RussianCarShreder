using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntryUI : MonoBehaviour
{
    public Text roomNameText;
    public Text playerCountText;
    public Image passwordIcon;

    [Header("UI Elements")]
    public Button joinButton;

    private string _roomName;
    private bool _hasPassword;

    public void Setup(string roomName, int currentPlayers, int maxPlayers, bool hasPassword)
    {
        _roomName = roomName;
        _hasPassword = hasPassword;

        if (roomNameText != null)
            roomNameText.text = roomName;

        if (playerCountText != null)
            playerCountText.text = $"{currentPlayers}/{maxPlayers}";

        if (passwordIcon != null)
            passwordIcon.gameObject.SetActive(hasPassword);

        if (joinButton != null)
            joinButton.onClick.AddListener(OnJoinButtonClick);
    }

    private void OnJoinButtonClick()
    {
        // »спользуем публичный метод дл€ выбора комнаты
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.SelectRoom(_roomName, _hasPassword);
        }
    }

    void OnDestroy()
    {
        if (joinButton != null)
            joinButton.onClick.RemoveAllListeners();
    }
}