//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class PhysicsParticle : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
//{
//    [Header("Particle Settings")]
//    [SerializeField] private Image particleImage;
//    [SerializeField] private float lifetime = 10f;
//    [SerializeField] private bool destroyOnClick = true;

//    [Header("Interaction Settings")]
//    [SerializeField] private float clickForce = 200f;
//    [SerializeField] private float hoverForce = 50f;

//    private Rigidbody2D rb;
//    private Collider2D particleCollider;
//    private float lifeTimer;

//    public System.Action OnParticleDestroyed;

//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        particleCollider = GetComponent<Collider2D>();
//        lifeTimer = lifetime;

//        // Настройка случайного цвета
//        if (particleImage != null)
//        {
//            particleImage.color = new Color(
//                Random.Range(0.5f, 1f),
//                Random.Range(0.5f, 1f),
//                Random.Range(0.5f, 1f),
//                0.8f
//            );
//        }
//    }

//    private void Update()
//    {
//        lifeTimer -= Time.deltaTime;

//        if (lifeTimer <= 0f)
//        {
//            DestroyParticle();
//        }
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        // Легкое отталкивание при наведении
//        if (rb != null)
//        {
//            Vector2 forceDirection = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
//            rb.AddForce(forceDirection * hoverForce);
//        }
//    }

//    public void OnPointerDown(PointerEventData eventData)
//    {
//        if (destroyOnClick)
//        {
//            DestroyParticle();
//        }
//        else
//        {
//            // Сильное отталкивание при клике
//            if (rb != null)
//            {
//                Vector2 forceDirection = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
//                rb.AddForce(forceDirection * clickForce, ForceMode2D.Impulse);
//            }
//        }
//    }

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        // Изменение цвета при столкновении
//        if (particleImage != null)
//        {
//            particleImage.color = Color.Lerp(particleImage.color, Color.white, 0.3f);
//        }
//    }

//    private void DestroyParticle()
//    {
//        OnParticleDestroyed?.Invoke();
//        Destroy(gameObject);
//    }

//    public void SetDestroyOnClick(bool shouldDestroy)
//    {
//        destroyOnClick = shouldDestroy;
//    }
//}