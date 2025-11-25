//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class PhysicsDemoManager : MonoBehaviour
//{
//    [Header("UI References")]
//    [SerializeField] private Button enablePhysicsButton;
//    [SerializeField] private Button disablePhysicsButton;
//    [SerializeField] private Button clearParticlesButton;
//    [SerializeField] private TextMeshProUGUI physicsStatusText;
//    [SerializeField] private Slider physicsIntensitySlider;

//    [Header("Physics Components")]
//    [SerializeField] private InteractiveBackground backgroundPhysics;
//    [SerializeField] private PhysicsButton[] physicsButtons;
//    [SerializeField] private UIZoneTrigger[] triggerZones;

//    [Header("Demo Objects")]
//    [SerializeField] private GameObject physicsParticlePrefab;

//    private bool physicsEnabled = true;

//    private void Start()
//    {
//        // ��������� ������
//        if (enablePhysicsButton != null)
//            enablePhysicsButton.onClick.AddListener(EnablePhysics);

//        if (disablePhysicsButton != null)
//            disablePhysicsButton.onClick.AddListener(DisablePhysics);

//        if (clearParticlesButton != null)
//            clearParticlesButton.onClick.AddListener(ClearParticles);

//        if (physicsIntensitySlider != null)
//            physicsIntensitySlider.onValueChanged.AddListener(OnPhysicsIntensityChanged);

//        // ��������� ���������� ���
//        foreach (var zone in triggerZones)
//        {
//            zone.OnObjectEnter.AddListener(() => OnZoneEnter(zone.gameObject.name));
//            zone.OnObjectExit.AddListener(() => OnZoneExit(zone.gameObject.name));
//        }

//        UpdatePhysicsStatus();
//    }

//    public void EnablePhysics()
//    {
//        physicsEnabled = true;
//        UpdatePhysicsComponents(true);
//        UpdatePhysicsStatus();
//    }

//    public void DisablePhysics()
//    {
//        physicsEnabled = false;
//        UpdatePhysicsComponents(false);
//        UpdatePhysicsStatus();
//    }

//    public void ClearParticles()
//    {
//        if (backgroundPhysics != null)
//        {
//            backgroundPhysics.ClearAllParticles();
//        }
//    }

//    private void UpdatePhysicsComponents(bool enabled)
//    {
//        // ���������/���������� ������ ��� ������
//        foreach (var physicsButton in physicsButtons)
//        {
//            if (physicsButton != null)
//            {
//                Rigidbody2D rb = physicsButton.GetComponent<Rigidbody2D>();
//                if (rb != null)
//                {
//                    rb.bodyType = enabled ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
//                }
//            }
//        }

//        // ���������/���������� ������� ������
//        if (backgroundPhysics != null)
//        {
//            backgroundPhysics.enabled = enabled;
//        }
//    }

//    private void OnPhysicsIntensityChanged(float intensity)
//    {
//        // ��������� ������������� ������ ����� �������
//        Time.timeScale = intensity;

//        foreach (var physicsButton in physicsButtons)
//        {
//            if (physicsButton != null)
//            {
//                Rigidbody2D rb = physicsButton.GetComponent<Rigidbody2D>();
//                if (rb != null)
//                {
//                    rb.linearDamping = 5f + (1f - intensity) * 10f;
//                }
//            }
//        }
//    }

//    private void OnZoneEnter(string zoneName)
//    {
//        Debug.Log($"������ ����� � ����: {zoneName}");

//        // ���������� �������� �����
//        if (physicsStatusText != null)
//        {
//            physicsStatusText.text = $"�������� ����: {zoneName}";
//            physicsStatusText.color = Color.green;
//        }
//    }

//    private void OnZoneExit(string zoneName)
//    {
//        Debug.Log($"������ ����� �� ����: {zoneName}");

//        if (physicsStatusText != null)
//        {
//            physicsStatusText.text = physicsEnabled ? "������ ��������" : "������ ���������";
//            physicsStatusText.color = physicsEnabled ? Color.white : Color.gray;
//        }
//    }

//    private void UpdatePhysicsStatus()
//    {
//        if (physicsStatusText != null)
//        {
//            physicsStatusText.text = physicsEnabled ? "������ ��������" : "������ ���������";
//            physicsStatusText.color = physicsEnabled ? Color.white : Color.gray;
//        }
//    }

//    // ������������ ������ ����� �����������
//    public void SpawnParticleWithCollider(int colliderType)
//    {
//        if (physicsParticlePrefab == null) return;

//        GameObject particle = Instantiate(physicsParticlePrefab);

//        // ������� ������ ���������
//        Collider2D oldCollider = particle.GetComponent<Collider2D>();
//        if (oldCollider != null)
//        {
//            Destroy(oldCollider);
//        }

//        // ��������� ����� ��������� � ����������� �� ����
//        switch (colliderType)
//        {
//            case 0: // Circle
//                particle.AddComponent<CircleCollider2D>();
//                break;
//            case 1: // Box
//                particle.AddComponent<BoxCollider2D>();
//                break;
//            case 2: // Capsule
//                CapsuleCollider2D capsule = particle.AddComponent<CapsuleCollider2D>();
//                capsule.direction = CapsuleDirection2D.Vertical;
//                break;
//        }
//    }
//}