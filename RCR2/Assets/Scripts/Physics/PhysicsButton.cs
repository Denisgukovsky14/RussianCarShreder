//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using System.Collections;

//[RequireComponent(typeof(Button), typeof(Rigidbody2D), typeof(Collider2D))]
//public class PhysicsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
//{
//    [Header("Physics Settings")]
//    [SerializeField] private float hoverForce = 25f;
//    [SerializeField] private float clickForce = 100f;
//    [SerializeField] private float returnSpeed = 5f;

//    [Header("Visual Settings")]
//    [SerializeField] private float hoverScale = 1.1f;
//    [SerializeField] private float clickScale = 0.95f;

//    private Button button;
//    private Rigidbody2D rb;
//    private RectTransform rectTransform;
//    private Vector2 originalPosition;
//    private Vector3 originalScale;

//    private bool isHovered = false;
//    private bool isPressed = false;

//    private void Awake()
//    {
//        button = GetComponent<Button>();
//        rb = GetComponent<Rigidbody2D>();
//        rectTransform = GetComponent<RectTransform>();

//        originalPosition = rectTransform.anchoredPosition;
//        originalScale = rectTransform.localScale;

//        // ��������� Rigidbody
//        rb.bodyType = RigidbodyType2D.Dynamic;
//        rb.gravityScale = 0f;
//        rb.linearDamping = 5f;
//        rb.angularDamping = 1f;
//        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//    }

//    private void Update()
//    {
//        if (!isHovered && !isPressed)
//        {
//            // ������� ����������� � �������� �������
//            rectTransform.anchoredPosition = Vector2.Lerp(
//                rectTransform.anchoredPosition,
//                originalPosition,
//                returnSpeed * Time.deltaTime
//            );
//        }

//        // ������� ����������� � ��������� ��������
//        if (!isPressed)
//        {
//            rectTransform.localScale = Vector3.Lerp(
//                rectTransform.localScale,
//                isHovered ? originalScale * hoverScale : originalScale,
//                returnSpeed * Time.deltaTime
//            );
//        }
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        if (!button.interactable) return;

//        isHovered = true;

//        // ������ ������������ �� �������
//        Vector2 forceDirection = (rectTransform.position - (Vector3)eventData.position).normalized;
//        rb.AddForce(forceDirection * hoverForce);

//        // ���������� �������� �����
//        StartCoroutine(ScaleAnimation(originalScale * hoverScale));
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        isHovered = false;
//        StartCoroutine(ScaleAnimation(originalScale));
//    }

//    public void OnPointerDown(PointerEventData eventData)
//    {
//        if (!button.interactable) return;

//        isPressed = true;

//        // ������� �������
//        Vector2 forceDirection = (rectTransform.position - (Vector3)eventData.position).normalized;
//        rb.AddForce(forceDirection * clickForce, ForceMode2D.Impulse);

//        // ���������� �������� �����
//        StartCoroutine(ScaleAnimation(originalScale * clickScale));
//    }

//    public void OnPointerUp(PointerEventData eventData)
//    {
//        isPressed = false;
//        StartCoroutine(ScaleAnimation(isHovered ? originalScale * hoverScale : originalScale));
//    }

//    private IEnumerator ScaleAnimation(Vector3 targetScale)
//    {
//        float duration = 0.1f;
//        float elapsed = 0f;
//        Vector3 startScale = rectTransform.localScale;

//        while (elapsed < duration)
//        {
//            elapsed += Time.deltaTime;
//            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
//            yield return null;
//        }

//        rectTransform.localScale = targetScale;
//    }

//    // ����� ��� ������ ������� (��������, ���� ������ "�������" �� �����)
//    public void ResetPosition()
//    {
//        rb.linearVelocity = Vector2.zero;
//        rb.angularVelocity = 0f;
//        rectTransform.anchoredPosition = originalPosition;
//        rectTransform.localScale = originalScale;
//    }
//}