using UnityEngine;

public class ExhaustPipe : MonoBehaviour
{
    [Header("Ссылки")]
    public ParticleSystem smokeParticles;

    [Header("Настройки дыма")]
    // Эмиссия - поток частиц
    public float minEmission = 5f;
    public float maxEmission = 5f;
    [Range(0, 1)] public float currentIntensity = 0f;

    [Header("Эффект движения")]
    public float velocityInfluence = 2f;
    public float maxSideVelocity = 3f;

    private Rigidbody carRigidbody;

    void Start()
    {
        carRigidbody = GetComponentInParent<Rigidbody>();
    }

    void Update()
    {
        UpdateSmoke();
        if (carRigidbody != null)
        {
            UpdateSmokeVelocity();
        }
    }

    void UpdateSmoke()
    {
        if (smokeParticles == null) return;

        var emission = smokeParticles.emission;
        float emissionRate = Mathf.Lerp(minEmission, maxEmission, currentIntensity);
        emission.rateOverTime = emissionRate;
    }

    void UpdateSmokeVelocity()
    {
        if (smokeParticles == null || carRigidbody == null) return;

        // Получаем локальную скорость машины
        // Если машина повернута на 90° вправо: (10, 0, 0) - теперь "вперед" это по оси X
        Vector3 localVelocity = transform.InverseTransformDirection(carRigidbody.linearVelocity);

        // Вычисляем влияние скорости на дым
        float sideVelocity = Mathf.Clamp(localVelocity.x * velocityInfluence, -maxSideVelocity, maxSideVelocity);
        float forwardVelocity = Mathf.Clamp(localVelocity.z * 0.5f, -1f, 2f);

        // Настраиваем Velocity over Lifetime
        var velocityModule = smokeParticles.velocityOverLifetime;
        velocityModule.enabled = true;

        //  ИСПРАВЛЕНИЕ: меняем оси местами
        velocityModule.x = sideVelocity;       // Боковое смещение
        velocityModule.y = 0f;                 // Убираем вертикаль
        velocityModule.z = 2f + forwardVelocity; // Основное направление + вперед/назад
    }

    public void SetSmokeIntensity(float intensity)
    {
        currentIntensity = Mathf.Clamp01(intensity);
    }
}