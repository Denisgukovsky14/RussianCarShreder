using UnityEngine;
using UnityEngine.UI;

public class RoomEntryUI : MonoBehaviour
{
    public Text roomNameText;
    public Text playerCountText;
    public Image passwordIcon;

    public void Setup(string roomName, int currentPlayers, int maxPlayers, bool hasPassword)
    {
        if (roomNameText != null)
            roomNameText.text = roomName;

        if (playerCountText != null)
            playerCountText.text = $"{currentPlayers}/{maxPlayers}";

        if (passwordIcon != null)
            passwordIcon.gameObject.SetActive(hasPassword);
    }
}