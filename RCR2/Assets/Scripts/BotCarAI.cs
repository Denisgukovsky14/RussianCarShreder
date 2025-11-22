using System.Collections.Generic;
using UnityEngine;

public class BotCarAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform playerTarget;
    public Vector3 targetDestination;

    [Header("AI Parameters")]
    public float attackDistance = 15f;
    public float rammingSpeed = 1f;
    public float maxSteerAngle = 30f;
    public float reverseDistance = 3f;
    public bool isDebug = false;
    private float lastReverseTime = 0f;
    private float timer = 0f;
    private float UnstackSpeed = 5.0f ;

    private float stuckTimer = 0f;
    private float stuckCheckTime = 7f; // Считаем застрявшей если 2 секунды скорость < 5

    [Header("Components")]
    private RearWheelDrive carController;
    private Rigidbody rb;

    void Start()
    {
        //isDebug = true;
        Debug.Log("=== BOT CAR AI START ===");

        // Находим компоненты
        carController = GetComponent<RearWheelDrive>();
        rb = GetComponent<Rigidbody>();

        if (carController == null)
        {
            Debug.LogError("RearWheelDrive component not found on bot!");
            enabled = false;
            return;
        }

        // Включаем AI управление
        carController.useAIControl = true;
        Debug.Log($"AI control set to: {carController.useAIControl}");

        // Если цель не назначена - ищем игрока
        if (playerTarget == null)
        {
            FindPlayerTarget();
        }
    }

    void Update()
    {

        Debug.Log($"=== BOT UPDATE ===");
        Debug.Log($"PlayerTarget: {playerTarget != null}");
        Debug.Log($"CarController: {carController != null}");
        Debug.Log($"useAIControl: {carController?.useAIControl}");
        Debug.Log($"AI Input - H: {carController?.aiHorizontal}, V: {carController?.aiVertical}");


        

        if (playerTarget == null)
        {
            FindPlayerTarget();
            return;
        }

        CalculateSteering();
        Debug.Log($"After CalculateSteering - H: {carController.aiHorizontal}, V: {carController.aiVertical}");


        if (isDebug)
        {
            DrawDebugInfo();
        }
    }

    void OnDestroy()
    {
        // При уничтожении выключаем AI управление
        if (carController != null)
        {
            carController.useAIControl = false;
        }
    }

    void FindPlayerTarget()
    {
        GameObject player = FindRandomObjectWithTagStartingWithPButNotPlatform();
        if (player != null)
        {
            playerTarget = player.transform;
            Debug.Log("Bot found player target: " + playerTarget.name);
        }
    }

    GameObject FindRandomObjectWithTagStartingWithPButNotPlatform()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> candidates = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.tag.StartsWith("P") && obj.tag != "Platform")
            {
                candidates.Add(obj);
            }
        }

        if (candidates.Count == 0)
            return null;

        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }

    void CalculateSteering()
    {

        if (playerTarget == null) return;

        Vector3 targetPoint = playerTarget.position;
        Vector3 toTarget = targetPoint - transform.position;
        toTarget.y = 0;

        // Направление машины
        Vector3 carForward = transform.forward;
        carForward.y = 0;

        // Угол к цели - ГЛАВНЫЙ ДИРЕКТОР ВЫЧИСЛЯЮЩИЙ УГОЛ ПОВОРОТА К ИГРОКУ
        float targetAngleDeg = Vector3.SignedAngle(carForward, toTarget.normalized, Vector3.up);

        // Если игрок в задней полусфере, едем назад

        if (Mathf.Abs(targetAngleDeg) < 140f)
        {
            // Игрок спереди - едем вперед
            carController.aiVertical = 0.8f;
            carController.aiHorizontal = Mathf.Clamp(targetAngleDeg / maxSteerAngle, -1f, 1f);
        }
        else
        {
            // Игрок сзади - едем назад
            carController.aiVertical = -0.6f;
            carController.aiHorizontal = Mathf.Clamp(targetAngleDeg / maxSteerAngle, -0.1f, 0.1f);
        }

        if (GetCurrentSpeed() < UnstackSpeed)
        {
            timer += Time.deltaTime;
            Debug.Log(timer);
            if (timer > 3.0f)
            {
                //UnstackSpeed += 2.0f;
                carController.aiVertical = -0.6f;
                carController.aiHorizontal = Mathf.Clamp(targetAngleDeg / maxSteerAngle, -0.1f, 0.1f);
            }
        }
        else
        {
            timer = 0.0f;
            UnstackSpeed = 5.0f;
        }

        Debug.Log($" Angle: {targetAngleDeg:F0}° | Speed: {carController.GetCurrentSpeed():F1} | Input: V:{carController.aiVertical:F1} H:{carController.aiHorizontal:F1}");
    }

    bool ShouldReverseAndTurn(float angle, float distance)
    {
        // Игрок почти прямо сзади и близко
        bool isDirectlyBehind = Mathf.Abs(angle) > 150f && distance < 10f;

        // Игрок сбоку-сзади и очень близко  
        bool isSideBehind = Mathf.Abs(angle) > 100f && distance < 5f;

        // Долго едем задним ходом (предотвращает бесконечный реверс)
        bool hasBeenReversing = Time.time - lastReverseTime < 3f;

        return (isDirectlyBehind || isSideBehind) && !hasBeenReversing;
    }

    // Добавь поле в класс:

 
    void HandleReverseTurn(float angle)
    {
        Debug.Log(" Performing reverse turn!");

        lastReverseTime = Time.time;

        float turnDirection = angle > 0 ? -1f : 1f;
        carController.aiHorizontal = turnDirection * 0.8f;
        carController.aiVertical = -0.6f;

        Invoke("ResetAfterReverse", 1.5f);
    }

    float CalculateThrottle(float distance, float angle)
    {
        // Если угол слишком большой - замедляемся для поворота
        float absAngle = Mathf.Abs(angle);
        float speedFactor = 1f;

        if (absAngle > 60f)
        {
            speedFactor = 0.3f; // Медленный поворот
        }
        else if (absAngle > 30f)
        {
            speedFactor = 0.7f; // Средняя скорость
        }

        // Режим реверса если цель сзади и близко
        if (absAngle > 100f && distance < reverseDistance)
        {
            return -0.5f; // Задний ход
        }

        // Основная логика скорости
        if (distance > attackDistance)
        {
            return 1f * speedFactor; // Полный газ
        }
        else if (distance > 5f)
        {
            return rammingSpeed * speedFactor; // Скорость тарана
        }
        else if (distance > 2f)
        {
            return 0.5f * speedFactor; // Средняя скорость
        }
        else
        {
            return 0.3f; // Медленное движение
        }
    }

    void DrawDebugInfo()
    {
        if (playerTarget != null)
        {
            // Линия к цели
            Debug.DrawLine(transform.position, playerTarget.position, Color.red);

            // Направление движения
            Debug.DrawRay(transform.position, transform.forward * 2f, Color.green);

            // Вектор управления
            Vector3 steeringVector = (transform.forward * carController.aiVertical +
                                    transform.right * carController.aiHorizontal) * 3f;
            Debug.DrawRay(transform.position, steeringVector, Color.yellow);
        }
    }

    // Для настройки из других скриптов
    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }

    public void SetDestination(Vector3 destination)
    {
        targetDestination = destination;
    }

    // Получение текущей скорости
    public float GetCurrentSpeed()
    {
        return rb != null ? rb.linearVelocity.magnitude * 3.6f : 0f;
    }
}