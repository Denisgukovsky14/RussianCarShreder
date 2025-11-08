using TMPro;
using UnityEngine;
using Photon.Pun;

public class SpeedMeter : MonoBehaviour
{
    private RearWheelDrive drivescript;
    private PhotonView view;
    float angle = 0;

    private void Start()
    {
        // СНАЧАЛА находим PhotonView
        GameObject car = transform.root.gameObject;
        view = car.GetComponent<PhotonView>();

        Debug.Log("SpeedMeter Start: view = " + (view != null ? "found" : "null"));

        if (view != null)
        {
            Debug.Log("IsMine = " + view.IsMine);

            if (view.IsMine)
            {
                // Настраиваем только для своего игрока
                drivescript = car.GetComponent<RearWheelDrive>();
                this.transform.eulerAngles = new Vector3(0, 0, angle);
                Debug.Log("SpeedMeter настроен для владельца");
            }
            else
            {
                // Отключаем для чужих игроков
                gameObject.SetActive(false);
                Debug.Log("SpeedMeter отключен для чужого игрока");
                return;
            }
        }
        else
        {
            Debug.LogError("PhotonView не найден на родительском объекте!");
            gameObject.SetActive(false);
            return;
        }
    }

    private void Update()
    {
        // Проверяем владение машиной
        if (view != null && view.IsMine && drivescript != null)
        {
            angle = (int)Mathf.Round(Mathf.Abs(drivescript.GetCurrentSpeed()) * -1.8f);
            this.transform.eulerAngles = new Vector3(0, 0, angle + 90);
        }
    }
}