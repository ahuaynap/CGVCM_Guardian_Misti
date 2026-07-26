using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class EarthquakeProtectionTrigger : MonoBehaviour
{
    public const string RequiredObjectiveId = GameIds.Level01Protect;
    public const float MinimumEntranceClearance = 1.01f;

    [SerializeField] private EarthquakeController earthquakeController;
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private NotificationUI notificationUI;
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TMP_Text contextualPrompt;
    [SerializeField] private TMP_Text objectiveHint;
    [SerializeField] private GameObject objectiveIndicator;
    [SerializeField] private Renderer floorBoundaryRenderer;
    [SerializeField] private Renderer statusLightRenderer;
    [SerializeField] private Light accentLight;
    [SerializeField] private string objectiveId = RequiredObjectiveId;
    [SerializeField, Min(.25f)] private float requiredDwellSeconds = 2f;
    [SerializeField, Min(1f)] private float promptDistance = 4.5f;
    [SerializeField, Min(1f)] private float hintDuration = 6f;

    private bool isInside;
    private bool dwellSatisfied;
    private float dwellProgress;
    private float hintRemaining;
    private MaterialPropertyBlock propertyBlock;
    private Color inactiveColor = new(.08f, .35f, .4f);
    private static readonly Color ActiveColor = new(.05f, .8f, .95f);
    private static readonly Color SuccessColor = new(.15f, .9f, .4f);

    public string ObjectiveId => objectiveId;
    public float RequiredDwellSeconds => requiredDwellSeconds;
    public float DwellProgress => dwellProgress;
    public bool IsInside => isInside;
    public bool DwellSatisfied => dwellSatisfied;
    public bool IsHighlighted { get; private set; }

    private void Awake()
    {
        var trigger = GetComponent<BoxCollider>();
        trigger.isTrigger = true;
        propertyBlock = new MaterialPropertyBlock();
        if (floorBoundaryRenderer != null) inactiveColor = floorBoundaryRenderer.sharedMaterial.color;
        SetGuidance(false, false);
    }

    private void OnEnable()
    {
        if (earthquakeController != null) earthquakeController.StateChanged += HandleEarthquakeState;
        if (objectivesManager != null) objectivesManager.ObjectiveChanged += HandleObjectiveChanged;
        RefreshRelevance();
    }

    private void Update()
    {
        bool relevant = IsProtectionObjectiveCurrent();
        bool active = relevant && earthquakeController != null && earthquakeController.IsProtectionPhase;
        if (active && isInside && !earthquakeController.ProtectionReached) earthquakeController.TryMarkProtectionEntered();
        if (active && isInside && !dwellSatisfied)
        {
            dwellProgress = CalculateDwellProgress(dwellProgress, Time.deltaTime, true, earthquakeController.State, requiredDwellSeconds);
            if (dwellProgress >= requiredDwellSeconds)
            {
                dwellSatisfied = true;
                earthquakeController.MarkProtectionDwellSatisfied();
                notificationUI?.ShowMessage("Protección confirmada", "Permanece bajo la mesa hasta que termine el sismo.");
                ApplyVisualColor(SuccessColor);
            }
        }
        UpdateGuidance(relevant, active);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isInside = true;
        if (player == null) player = other.transform;
        if (earthquakeController == null || !earthquakeController.TryMarkProtectionEntered()) return;
        notificationUI?.ShowMessage("Zona de protección alcanzada", "Permanece bajo la mesa.");
        HidePromptAndHint();
        ApplyVisualColor(SuccessColor);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isInside = false;
        earthquakeController?.MarkProtectionExited();
        if (dwellSatisfied) return;
        dwellProgress = 0f;
        if (earthquakeController != null && earthquakeController.IsProtectionPhase)
            notificationUI?.ShowMessage("Protección incompleta", "Permanece en la zona de protección.");
    }

    public static bool IsEligibleState(EarthquakeState state) =>
        state is EarthquakeState.Light or EarthquakeState.Moderate or EarthquakeState.Strong;

    public static float CalculateDwellProgress(float current, float deltaTime, bool inside, EarthquakeState state, float required)
    {
        if (!inside || !IsEligibleState(state)) return 0f;
        return Mathf.Min(Mathf.Max(.25f, required), Mathf.Max(0f, current) + Mathf.Max(0f, deltaTime));
    }

    private bool IsProtectionObjectiveCurrent() =>
        objectivesManager != null && objectivesManager.IsCurrentObjective(objectiveId);

    private void HandleEarthquakeState(EarthquakeState state)
    {
        if (state == EarthquakeState.Light) hintRemaining = hintDuration;
        if (!IsEligibleState(state) && !dwellSatisfied) dwellProgress = 0f;
        RefreshRelevance();
    }

    private void HandleObjectiveChanged(Objective objective) => RefreshRelevance();

    private void RefreshRelevance()
    {
        bool relevant = IsProtectionObjectiveCurrent();
        if (!relevant)
        {
            HidePromptAndHint();
            SetGuidance(false, false);
            return;
        }
        if (earthquakeController != null && earthquakeController.State == EarthquakeState.Light) hintRemaining = hintDuration;
    }

    private void UpdateGuidance(bool relevant, bool active)
    {
        if (!relevant)
        {
            HidePromptAndHint();
            SetGuidance(false, false);
            return;
        }
        if (hintRemaining > 0f && !isInside)
        {
            hintRemaining = Mathf.Max(0f, hintRemaining - Time.deltaTime);
            SetText(objectiveHint, "Busca la zona señalizada bajo la mesa.", true);
        }
        else SetText(objectiveHint, string.Empty, false);
        float distance = player == null ? float.MaxValue : Vector3.Distance(player.position, transform.position);
        SetText(contextualPrompt, "Entra en la zona de protección", active && !dwellSatisfied && distance <= promptDistance);
        bool visible = playerCamera != null && IsVisibleFrom(playerCamera);
        bool showIndicator = active && !dwellSatisfied && distance > 2.2f && !visible;
        if (objectiveIndicator != null) objectiveIndicator.SetActive(showIndicator);
        SetGuidance(active && !dwellSatisfied, dwellSatisfied);
    }

    private bool IsVisibleFrom(Camera camera)
    {
        Vector3 viewport = camera.WorldToViewportPoint(transform.position + Vector3.up);
        return viewport.z > 0f && viewport.x > .08f && viewport.x < .92f && viewport.y > .12f && viewport.y < .88f;
    }

    private void SetGuidance(bool active, bool success)
    {
        IsHighlighted = active;
        if (accentLight != null)
        {
            accentLight.enabled = active || success;
            accentLight.color = success ? SuccessColor : ActiveColor;
            accentLight.intensity = success ? 2f : 1.3f + Mathf.Sin(Time.time * 2f) * .15f;
        }
        ApplyVisualColor(success ? SuccessColor : active ? ActiveColor : inactiveColor);
        if (objectiveIndicator != null && !active) objectiveIndicator.SetActive(false);
    }

    private void ApplyVisualColor(Color color)
    {
        ApplyRendererColor(floorBoundaryRenderer, color);
        ApplyRendererColor(statusLightRenderer, color);
    }

    private void ApplyRendererColor(Renderer target, Color color)
    {
        if (target == null) return;
        target.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", color);
        propertyBlock.SetColor("_EmissionColor", color * 1.4f);
        target.SetPropertyBlock(propertyBlock);
    }

    private void HidePromptAndHint()
    {
        SetText(contextualPrompt, string.Empty, false);
        SetText(objectiveHint, string.Empty, false);
    }

    private static void SetText(TMP_Text text, string value, bool visible)
    {
        if (text == null) return;
        text.text = visible ? value : string.Empty;
        text.gameObject.SetActive(visible);
    }

    private void OnDisable()
    {
        if (earthquakeController != null) earthquakeController.StateChanged -= HandleEarthquakeState;
        if (objectivesManager != null) objectivesManager.ObjectiveChanged -= HandleObjectiveChanged;
        HidePromptAndHint();
        SetGuidance(false, false);
    }
}
