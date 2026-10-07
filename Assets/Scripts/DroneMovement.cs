using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float ascendSpeed = 2f;
    public float descendSpeed = 2f;
    private Camera camera;

    void Start()
    {
        SetupSpectatorCamera();
    }

    void SetupSpectatorCamera()
    {
        // Создаем или находим камеру для наблюдения
        GameObject cameraObj = GameObject.Find("SpectatorCamera");
        if (cameraObj == null)
        {
            cameraObj = new GameObject("SpectatorCamera");
            camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
        }
        else
        {
            camera = cameraObj.GetComponent<Camera>();
        }

        // Прикрепляем камеру к дрону
        cameraObj.transform.SetParent(transform);
        cameraObj.transform.localPosition = new Vector3(0, 0.3f, 0);
        cameraObj.transform.localRotation = Quaternion.identity;

        // Отключаем другие камеры если нужно
        DisablePlayerCameras();
    }

    void DisablePlayerCameras()
    {
        // Отключаем камеры живых игроков
        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            if (cam != camera)
            {
                cam.enabled = false;
                AudioListener audioListener = cam.GetComponent<AudioListener>();
                if (audioListener != null) audioListener.enabled = false;
            }
        }
    }

    void Update()
    {
        // Управление дроном (только для наблюдателя)
        HandleMovement();
    }

    void HandleMovement()
    {
        // Твое текущее управление...
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;

        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);

        // Подъем/опускание
        if (Input.GetKey(KeyCode.Space)) transform.Translate(Vector3.up * ascendSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.LeftShift)) transform.Translate(Vector3.down * descendSpeed * Time.deltaTime);

        // Поворот камеры (мышью)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(0, mouseX * 2f, 0);
        camera.transform.Rotate(-mouseY * 2f, 0, 0);
    }
}