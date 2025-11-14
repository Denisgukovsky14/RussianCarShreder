using UnityEngine;

public class BillboardAd : MonoBehaviour
{
    [Header("Настройки билборда")]
    public AdType adType = AdType.Banner;
    public float showDistance = 20f;

    [Header("Визуальные настройки")]
    public Color normalColor = Color.white;     // Обычный цвет
    public Color activeColor = Color.green;     // Цвет когда игрок рядом
    public Color triggeredColor = Color.yellow; // Цвет после показа рекламы

    public enum AdType { Banner, Interstitial, Rewarded }

    private PlayerAdManager playerAdManager;
    private bool adShown = false;
    private Renderer billboardRenderer;

    void Start()
    {
        // Получаем компонент рендерера
        billboardRenderer = GetComponent<Renderer>();

        // Устанавливаем начальный цвет
        if (billboardRenderer != null)
            billboardRenderer.material.color = normalColor;

        // Делаем билборд плоским (опционально)
        MakeFlatBillboard();
    }

    void MakeFlatBillboard()
    {
        transform.localScale = new Vector3(10f, 5f, 0.1f);
    }

    void Update()
    {
        // Ищем PlayerAdManager
        if (playerAdManager == null)
        {
            playerAdManager = FindObjectOfType<PlayerAdManager>();
            return;
        }

        // Проверяем расстояние до игрока
        if (playerAdManager != null && playerAdManager.gameObject != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position,
                playerAdManager.transform.position);

            if (distanceToPlayer <= showDistance)
            {
                if (!adShown)
                {
                    // Игрок приблизился - меняем цвет и показываем рекламу
                    SetColor(activeColor);
                    ShowAd();
                    adShown = true;
                }
                else
                {
                    // Игрок всё ещё рядом - оставляем цвет активации
                    SetColor(triggeredColor);
                }
            }
            else
            {
                // Игрок уехал - возвращаем обычный цвет
                SetColor(normalColor);
                adShown = false;
            }
        }
        else
        {
            playerAdManager = null;
            SetColor(normalColor);
        }
    }

    void SetColor(Color color)
    {
        if (billboardRenderer != null)
            billboardRenderer.material.color = color;
    }

    void ShowAd()
    {
        if (playerAdManager == null) return;

        Debug.Log($"Билборд активирован! Тип: {adType}");

        switch (adType)
        {
            case AdType.Banner:
                playerAdManager.ShowBanner();
                break;
            case AdType.Interstitial:
                playerAdManager.ShowInterstitial();
                break;
            case AdType.Rewarded:
                playerAdManager.ShowRewarded();
                break;
        }

        // Мигаем когда реклама показана
        StartCoroutine(FlashBillboard());
    }

    System.Collections.IEnumerator FlashBillboard()
    {
        if (billboardRenderer == null) yield break;

        // Мигание при активации
        SetColor(Color.red);
        yield return new WaitForSeconds(0.3f);
        SetColor(triggeredColor);
    }
}