using UnityEngine;

public class Radar : MonoBehaviour
{
    // Скриптовая анимация

    [Header("Настройки вращения")]
    [Tooltip("Время полного оборота в секундах")]
    public float rotationTime = 5f;

    void Update()
    {
        // Делим расстояние на время, получаем скорость
        float speed = 360f / rotationTime;

        // Вращаем вокруг оси Y
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
