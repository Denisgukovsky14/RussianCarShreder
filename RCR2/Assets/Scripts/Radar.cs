using UnityEngine;

public class Radar : MonoBehaviour
{
    // Скриптовая анимация

    [Header("Настройки вращения")]
    [Tooltip("Время полного оборота в секундах")]
    public float rotationTime = 5f;

    void Update()
    {
        // Вычисляем скорость: 360 градусов / время
        float speed = 360f / rotationTime;

        // Вращаем вокруг оси Y
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
