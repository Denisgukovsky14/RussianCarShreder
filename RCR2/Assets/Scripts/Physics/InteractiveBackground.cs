//using UnityEngine;
//using UnityEngine.UI;

//public class InteractiveBackground : MonoBehaviour
//{
//    [Header("Particle Settings")]
//    [SerializeField] private GameObject physicsParticlePrefab;
//    [SerializeField] private int maxParticles = 20;
//    [SerializeField] private float spawnInterval = 0.5f;
//    [SerializeField] private RectTransform spawnArea;

//    [Header("Physics Settings")]
//    [SerializeField] private float minForce = 50f;
//    [SerializeField] private float maxForce = 150f;
//    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;

//    private Canvas canvas;
//    private float spawnTimer;
//    private int currentParticleCount = 0;

//    private void Start()
//    {
//        canvas = GetComponentInParent<Canvas>();
//        spawnTimer = spawnInterval;

//        // Предзагрузка нескольких частиц
//        PreloadParticles(5);
//    }

//    private void Update()
//    {
//        spawnTimer -= Time.deltaTime;

//        if (spawnTimer <= 0f && currentParticleCount < maxParticles)
//        {
//            SpawnParticle();
//            spawnTimer = spawnInterval;
//        }
//    }

//    private void PreloadParticles(int count)
//    {
//        for (int i = 0; i < count && currentParticleCount < maxParticles; i++)
//        {
//            SpawnParticle();
//        }
//    }

//    private void SpawnParticle()
//    {
//        if (physicsParticlePrefab == null || spawnArea == null) return;

//        Vector2 spawnPosition = GetRandomSpawnPosition();
//        GameObject particle = Instantiate(physicsParticlePrefab, spawnArea);

//        // Устанавливаем позицию
//        RectTransform particleRect = particle.GetComponent<RectTransform>();
//        particleRect.anchoredPosition = spawnPosition;

//        // Добавляем случайную силу
//        Rigidbody2D rb = particle.GetComponent<Rigidbody2D>();
//        if (rb != null)
//        {
//            Vector2 randomForce = new Vector2(
//                Random.Range(-1f, 1f),
//                Random.Range(-0.5f, 1f)
//            ).normalized * Random.Range(minForce, maxForce);

//            // rb.AddForce(randomForce, forceMode);
//        }

//        currentParticleCount++;

//        // Настраиваем автоматическое уничтожение
//        PhysicsParticle particleScript = particle.GetComponent<PhysicsParticle>();
//        if (particleScript != null)
//        {
//            particleScript.OnParticleDestroyed += OnParticleDestroyed;
//        }
//    }

//    private Vector2 GetRandomSpawnPosition()
//    {
//        Vector2 areaSize = spawnArea.rect.size;
//        return new Vector2(
//            Random.Range(-areaSize.x / 2, areaSize.x / 2),
//            Random.Range(-areaSize.y / 2, areaSize.y / 2)
//        );
//    }

//    private void OnParticleDestroyed()
//    {
//        currentParticleCount--;
//    }

//    public void ClearAllParticles()
//    {
//        PhysicsParticle[] particles = FindObjectsOfType<PhysicsParticle>();
//        foreach (PhysicsParticle particle in particles)
//        {
//            Destroy(particle.gameObject);
//        }
//        currentParticleCount = 0;
//    }
//}