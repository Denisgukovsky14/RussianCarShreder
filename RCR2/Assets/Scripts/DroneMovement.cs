using Photon.Pun;
using RootMotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DroneMovement : MonoBehaviour
{
    public float moveSpeed = 5f;     // Скорость движения по горизонтали
    public float ascendSpeed = 2f;   // Скорость подъема
    public float descendSpeed = 2f;  // Скорость опускания
    private PhotonView view;
    private Camera camera;

    public void Initialize ( PhotonView takeview )
    {
        view = takeview;
    }

    void Start()
    {
        Transform cameraTransform = transform.Find("Camera");
        camera = cameraTransform.GetComponent<Camera>();
        cameraTransform.GetComponent<CameraController>().enabled = true;
        camera.enabled = true;
        camera.transform.position += new Vector3(0, -0.3f, 0);
    }

    void Update()
    {
        if (camera == null) return; // Если камера не найдена, выходим из метода

        // Получаем направление камеры
        Vector3 cameraDirection = camera.transform.forward;
        cameraDirection.y = 0; // Игнорируем вертикальную компоненту для движения

        // Двигаем объект по направлению к камере
        Vector3 rightDirection = camera.transform.right;

        // Обрабатываем движение
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) // Вперед
        {
            moveDirection += cameraDirection.normalized;
        }
        if (Input.GetKey(KeyCode.S)) // Назад
        {
            moveDirection -= cameraDirection.normalized;
        }
        if (Input.GetKey(KeyCode.A)) // Влево
        {
            moveDirection -= rightDirection.normalized;
        }
        if (Input.GetKey(KeyCode.D)) // Вправо
        {
            moveDirection += rightDirection.normalized;
        }

        // Поворачиваем дрон в направлении камеры
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);

        // Поворачиваем дрон в направлении движения
        if (moveDirection != Vector3.zero) // Проверяем, чтобы избежать ошибок при нулевом векторе
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Плавный поворот
        }

        // Проверяем нажатие пробела для подъема
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * ascendSpeed * Time.deltaTime);
        }

        // Проверяем нажатие Shift для опускания
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(Vector3.down * descendSpeed * Time.deltaTime);
        }
    }
}
