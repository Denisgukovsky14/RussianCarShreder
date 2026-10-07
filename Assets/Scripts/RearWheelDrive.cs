using UnityEngine;
using System.Collections;
using Photon.Pun;
using RootMotion;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class RearWheelDrive : MonoBehaviour
{
    [Header("AI Control")]
    public bool useAIControl = false;
    public float aiHorizontal = 0f;
    public float aiVertical = 0f;

    public PlayerInfo MyplayerInfo;
    public GameObject canvas;
    private PlayerCanvas playerCanvas;
    public ExhaustPipe pipe;

    public CarConfig CarConfig;
    private PhotonView view;
    private WheelCollider[] wheels;
    [SerializeField] private FixedJoystick joystick;

    //public TextMeshProUGUI speedmeter;
    public float maxAngle = 30;
    public float maxTorque = 300;
    public float brakeTorque = 5000;
    public float handbrakeTorque = 10000;
    public float accelerationMultiplier = 1.5f;

    public bool firsthit = false;

    bool isHandbrakeActive = false;

    private float previousSpeed;
    private float currentSpeed;

    public GameObject wheelShape;
    public GameObject Camera;
    public GameObject Drone;
    public PlayerListUI playerListUI;
    private GameObject playerListPanel;

    private Rigidbody rb;

    public float targetAccelerationTime = 5f;
    private bool breakActivate = false;

    public float maxSpeed = 230f;
    public float maxReverseSpeed = 20f;
    public float accelerationCurve = 1.5f;
    public bool isListVisible;

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    //PunRpc это такой своеобразный серверный синхронизатор который всем игрокам показывает одинаковую картинку
    [PunRPC]
    public void UpdateWheelPose(Vector3 position, Quaternion rotation)
    {
        foreach (WheelCollider wheel in wheels)
        {
            Transform shapeTransform = wheel.transform.GetChild(0);
            shapeTransform.position = position;
            shapeTransform.rotation = rotation;
        }
    }

    private void CreateCanvasForPlayer()
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas Prefab не назначен!");
            return;
        }

       

        // Создаем канвас как дочерний объект этой машины
        GameObject canvasObject = Instantiate(canvas, transform);

        // Настраиваем позицию (если нужно)
        canvasObject.transform.localPosition = Vector3.zero;
        canvasObject.transform.localRotation = Quaternion.identity;

        playerCanvas = canvasObject.GetComponent<PlayerCanvas>();

        if (playerCanvas != null)
        {
            playerCanvas.SetupForPlayer(this);
            Debug.Log("Canvas создан как дочерний объект!");

            playerListUI = playerCanvas.GetComponentInChildren<PlayerListUI>();

            if (playerListUI != null)
            {
                playerListPanel = playerListUI.gameObject;
                playerListPanel.gameObject.SetActive(false);
            }

        }
        else
        {
            Debug.LogError("PlayerCanvas компонент не найден!");
        }
    }

    public void Start()
    {
        if ( transform.tag == "Bot")
        {
            useAIControl = true;
            Debug.Log("================================================");
            maxSpeed = 170f;
            maxTorque += 100;
        } 

        rb = GetComponentInParent<Rigidbody>();
        previousSpeed = rb.linearVelocity.magnitude;
        view = GetComponent<PhotonView>();

        if (CarConfig != null)
        {
            maxAngle = CarConfig.maxAngle;
            maxTorque = CarConfig.maxTorque;
            brakeTorque = CarConfig.brakeTorque;
            handbrakeTorque = CarConfig.handbrakeTorque;
            accelerationMultiplier = CarConfig.accelerationMultiplier;
            targetAccelerationTime = CarConfig.targetAccelerationTime;
            maxSpeed = CarConfig.maxSpeed;
            maxReverseSpeed = CarConfig.maxReverseSpeed;
            accelerationCurve = CarConfig.accelerationCurve;

            if (CarConfig.wheelShape != null)
            {
                wheelShape = CarConfig.wheelShape;
            }
        }

        if (view.IsMine || useAIControl)
        {
            Debug.Log("AGA");
            CreateCanvasForPlayer();
        }
            wheels = GetComponentsInChildren<WheelCollider>();

            for (int i = 0; i < wheels.Length; ++i)
            {
                var wheel = wheels[i];

                if (wheelShape != null)
                {
                    Vector3 wheelPosition;
                    Quaternion wheelRotation;
                    wheel.GetWorldPose(out wheelPosition, out wheelRotation);

                    var ws = GameObject.Instantiate(wheelShape);

                    ws.transform.parent = wheel.transform;
                    ws.transform.position = wheelPosition;
                    ws.transform.rotation = wheelRotation;
                    ws.transform.localScale = new Vector3(100f, 100f, 100f);

                    if (((i + 1) % 2) == 0)
                    {
                        ws.transform.localScale = new Vector3(ws.transform.localScale.x * -1, ws.transform.localScale.y, ws.transform.localScale.z);
                    }
                }
            }
        //}
    }

    public void FixedUpdate()
    {
        if (view.IsMine || useAIControl)
        {

            HandlePlayerListInput();

            currentSpeed = rb.linearVelocity.magnitude * 3.6f;
            
            //speedmeter.text = "Speed: " + Mathf.Round(currentSpeed);

            // Здесь мы задаем интенсивность дыма
            pipe.SetSmokeIntensity(Mathf.Max( currentSpeed/maxSpeed , Mathf.Abs( maxTorque * 1.5f) ) );

            float angle = 0f;
            float torque = 0f;


            //if (joystick)
            //{
            //    angle = maxAngle * joystick.Horizontal;
            //    torque = maxTorque * joystick.Vertical;
            //}

            //angle = maxAngle * (Input.GetAxis("Horizontal") + joystick.Horizontal);
            //torque = maxTorque * (Input.GetAxis("Vertical") + joystick.Vertical);

            float horizontal, vertical;

            if (useAIControl)
            {
                // Используем AI управление
                horizontal = Mathf.Clamp(aiHorizontal, -1f, 1f);
                vertical = Mathf.Clamp(aiVertical, -1f, 1f);
                //Debug.Log($"AI Driving - H: {horizontal}, V: {vertical}"); 
            }
            else
            {
                // Старая логика - управление игрока
                if (Mathf.Abs(joystick.Horizontal) > 0.1f || Mathf.Abs(joystick.Vertical) > 0.1f)
                {
                    horizontal = joystick.Horizontal;
                    vertical = joystick.Vertical;
                }
                else
                {
                    horizontal = Input.GetAxis("Horizontal");
                    vertical = Input.GetAxis("Vertical");
                }
            }

            // Дальше твой существующий код остается без изменений:
            angle = maxAngle * horizontal;
            torque = maxTorque * vertical;

            if (!breakActivate)
            {
                isHandbrakeActive = Input.GetKey(KeyCode.Space);
            }

            float speedRatio;
            speedRatio = Mathf.Abs(currentSpeed) / maxSpeed;
            
            float currentAccelerationMultiplier = accelerationMultiplier * (1f - Mathf.Pow(speedRatio, accelerationCurve));

            // Применяем мультипликатор ускорения
            float finalTorque = torque * currentAccelerationMultiplier;

            if (isHandbrakeActive) {
                torque -= 3.6f;
            }


            // Ограничение скорости вперед
            if (currentSpeed > maxSpeed && finalTorque > 0)
            {
                finalTorque = 0;
            }

            // Ограничение скорости назад
            float forwardVelocity = Vector3.Dot(rb.linearVelocity, transform.forward);
            if (forwardVelocity < -maxReverseSpeed / 4.0f && finalTorque < 0)
            {
                finalTorque = 0;
            }

            // Логика торможения при смене направления
            if (torque > 0 && forwardVelocity < -1f)
            {
                // Если едем назад и хотим вперед - тормозим
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.brakeTorque = brakeTorque;
                }
                finalTorque = 0;
            }
            else if (torque < 0 && forwardVelocity > 1f)
            {
                // Если едем вперед и хотим назад - тормозим
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.brakeTorque = brakeTorque;
                }
                finalTorque = 0;
            }
            else
            {
                // Снимаем тормоза если не нужно тормозить
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.brakeTorque = 0;
                }
            }

            // Применение ручного тормоза
            if (isHandbrakeActive)
            {
                foreach (WheelCollider wheel in wheels)
                {
                    if (wheel.transform.localPosition.z < 0) // Задние колеса
                    {
                        wheel.brakeTorque = handbrakeTorque;
                    }
                    //wheel.motorTorque = 0;
                    
                }
            }
            else
            {
                foreach (WheelCollider wheel in wheels)
                {
                    if (wheel.transform.localPosition.z < 0) // Задние колеса
                    {
                        wheel.motorTorque = finalTorque;
                    }

                    if (wheel.transform.localPosition.z > 0) // Передние колеса
                    {
                        wheel.steerAngle = angle;
                    }

                    // Обновление визуальных колес
                    //if (wheelShape)
                    //{
                    //    Quaternion q;
                    //    Vector3 p;
                    //    wheel.GetWorldPose(out p, out q);

                    //    Transform shapeTransform = wheel.transform.GetChild(0);
                    //    shapeTransform.position = p;
                    //    shapeTransform.rotation = q;
                    //}
                }
            }

            bool LetsDeath = Input.GetKey(KeyCode.V);
            if (LetsDeath)
            {

                Debug.Log("СМЭРТЬ");
                foreach (WheelCollider wheel in wheels)
                {
                    Destroy(wheel.gameObject);
                }

                Destroy(gameObject);
            }
        }
        foreach (WheelCollider wheel in wheels)
        {
            if (wheelShape)
            {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                Transform shapeTransform = wheel.transform.GetChild(0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
        }
    }

    void HandlePlayerListInput()
    {
        if (playerListPanel == null) return;

        if (Input.GetKey(KeyCode.Tab) && !isListVisible)
        {
            // ВКЛЮЧАЕМ
            playerListPanel.SetActive(true);
            isListVisible = true;
            Debug.Log(" Список показан");
        }
        else if (!Input.GetKey(KeyCode.Tab) && isListVisible)
        {
            // ВЫКЛЮЧАЕМ
            playerListPanel.SetActive(false);
            isListVisible = false;
            Debug.Log(" Список скрыт");
        }
    }

    public void OnHandbrakeButtonDown()
    {
        isHandbrakeActive = true;
        breakActivate = true;
    }

    public void OnHandbrakeButtonUp()
    {
        isHandbrakeActive = false;
        breakActivate = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (view.IsMine || useAIControl)
        {
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb != null && firsthit == true)
            {
                firsthit = false; // Сразу ставим false

                float otherMass = otherRb.mass;
                Vector3 otherVelocity = otherRb.linearVelocity;
                Vector3 impactForce = rb.linearVelocity * rb.mass * 5.6f + otherVelocity * otherMass * 5.6f;
                otherRb.AddForce(impactForce, ForceMode.Impulse);

                Debug.Log(" First collision handled");

                // Через 1 секунду сбрасываем флаг
                Invoke("ResetFirstHit", 1f);
            }
        }
    }

    private void ResetFirstHit()
    {
        firsthit = true;
        Debug.Log(" FirstHit reset after 1 second");
    }

    private void OnDestroy()
    {

        if (view.IsMine)
        {
            //MyplayerInfo.MyDeath(PhotonNetwork.NickName);

            gameObject.GetComponent<Explosion>().Explode();
            var drone = GameObject.Instantiate(Drone, transform.position, transform.rotation);
            drone.tag = transform.tag;
            Camera.transform.SetParent(drone.transform);
            Camera.transform.localPosition = Vector3.zero;
        }
        else
        {
            gameObject.GetComponent<Explosion>().Explode();
        }
        
    }
}