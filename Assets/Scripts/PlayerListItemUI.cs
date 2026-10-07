using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text statusText;
    //public TextMeshProUGUI idText;  // Опционально: для отображения ID
    public GameObject idText;  // Опционально: для отображения ID

    public void Setup(string nickname, int health, bool isAlive, int actorNumber)
    {
        UpdateInfo(nickname, health, isAlive, actorNumber);

        //if (idText2 != null)
        //    idText2.text = $"ID: {actorNumber}";
    }

    public void UpdateInfo(string nickname, int health, bool isAlive, int actorNumber)
    {
        idText.GetComponent<Text>().text = $"ID: {actorNumber}";
        idText.GetComponent<Text>().color = Color.white ;
        nameText.text = nickname;
        healthText.text = $"HP: {health}";
        statusText.text = isAlive ? "Alive" : "Dead";
        statusText.color = isAlive ? Color.white : Color.red;
        healthText.color = health > 50 ? Color.white : health > 20 ? Color.yellow : Color.red;
    }
}