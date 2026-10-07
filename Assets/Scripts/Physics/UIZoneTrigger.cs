//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;

//public class UIZoneTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//{
//    [Header("Trigger Events")]
//    public UnityEvent OnObjectEnter;
//    public UnityEvent OnObjectExit;

//    [Header("Visual Feedback")]
//    [SerializeField] private bool changeColorOnHover = true;
//    [SerializeField] private Color hoverColor = Color.yellow;

//    private Image zoneImage;
//    private Color originalColor;

//    private void Awake()
//    {
//        zoneImage = GetComponent<Image>();
//        if (zoneImage != null)
//        {
//            originalColor = zoneImage.color;
//        }
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        OnObjectEnter?.Invoke();

//        if (changeColorOnHover && zoneImage != null)
//        {
//            zoneImage.color = hoverColor;
//        }
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        OnObjectExit?.Invoke();

//        if (changeColorOnHover && zoneImage != null)
//        {
//            zoneImage.color = originalColor;
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        // Обработка входа физических объектов в триггер
//        PhysicsParticle particle = other.GetComponent<PhysicsParticle>();
//        if (particle != null)
//        {
//            Debug.Log($"Частица вошла в зону: {name}");
//            OnObjectEnter?.Invoke();
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        // Обработка выхода физических объектов из триггера
//        PhysicsParticle particle = other.GetComponent<PhysicsParticle>();
//        if (particle != null)
//        {
//            Debug.Log($"Частица вышла из зоны: {name}");
//            OnObjectExit?.Invoke();
//        }
//    }
//}