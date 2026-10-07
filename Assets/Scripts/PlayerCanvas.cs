using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine;

public class PlayerCanvas : MonoBehaviourPunCallbacks
{
    [Header("UI Elements")]
    public GameObject speedMeter;
    public FixedJoystick joystick;
    public GameObject brakeButton;

    private RearWheelDrive targetCar;

    // Вызывается когда канвас создан для игрока
    public void SetupForPlayer(RearWheelDrive car)
    {
        targetCar = car;

        // Настраиваем кнопки только для нашего игрока
        if (photonView.IsMine)
        {
            SetupControls();
        }
        else
        {
            // Для других игроков отключаем кнопки
            brakeButton.gameObject.SetActive(false);
        }
    }

    private void SetupControls()
    {
        speedMeter.GetComponent<SpeedMeter>().enabled = true;
        // Кнопка тормоза (зажатие/отпускание)
        EventTrigger brakeTrigger = brakeButton.gameObject.AddComponent<EventTrigger>();

        // Нажатие
        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => { targetCar.OnHandbrakeButtonDown(); });
        brakeTrigger.triggers.Add(pointerDown);

        // Отпускание
        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { targetCar.OnHandbrakeButtonUp(); });
        brakeTrigger.triggers.Add(pointerUp);

    }
}