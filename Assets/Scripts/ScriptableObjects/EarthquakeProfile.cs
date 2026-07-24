using UnityEngine;

[CreateAssetMenu(menuName = "Guardian Misti/Earthquake Profile")]
public sealed class EarthquakeProfile : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; } = "Básico";
    [field: SerializeField, Min(1f)] public float Duration { get; private set; } = 18f;
    [field: SerializeField, Min(0f)] public float PreparationCountdown { get; private set; } = 3f;
    [field: SerializeField] public AnimationCurve IntensityCurve { get; private set; } =
        new(new Keyframe(0, 0), new Keyframe(.25f, .45f), new Keyframe(.55f, 1), new Keyframe(1, 0));
    [field: SerializeField, Range(0, .2f)] public float CameraShakeIntensity { get; private set; } = .055f;
    [field: SerializeField, Range(1, 30)] public float CameraShakeFrequency { get; private set; } = 11f;
    [field: SerializeField] public float MaximumPropForce { get; private set; } = 2f;
    [field: SerializeField, Range(0, 1)] public float AudioIntensity { get; private set; } = .7f;
    [field: SerializeField, Range(0, 1)] public float LightFlickerIntensity { get; private set; } = .35f;
    [field: SerializeField, Min(20)] public float TargetSeconds { get; private set; } = 100f;

    public float Evaluate(float elapsed) => Duration <= 0 ? 0 : Mathf.Clamp01(IntensityCurve.Evaluate(Mathf.Clamp01(elapsed / Duration)));
}
