using UnityEngine;

[CreateAssetMenu(menuName = "Guardian Misti/Earthquake Profile")]
public sealed class EarthquakeProfile : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; } = "Básico";
    [field: SerializeField, Min(1f)] public float Duration { get; private set; } = 18f;
    [field: SerializeField, Min(0f)] public float PreparationCountdown { get; private set; } = 3f;
    [field: SerializeField] public AnimationCurve IntensityCurve { get; private set; } = DefaultCurve();
    [field: SerializeField] public AnimationCurve CameraShakeCurve { get; private set; } = DefaultCurve();
    [field: SerializeField] public AnimationCurve AudioVolumeCurve { get; private set; } = DefaultCurve();
    [field: SerializeField] public AnimationCurve LightFlickerCurve { get; private set; } = DefaultCurve();
    [field: SerializeField] public AnimationCurve DustEmissionCurve { get; private set; } = DefaultCurve();
    [field: SerializeField] public AnimationCurve PropForceCurve { get; private set; } = DefaultCurve();
    [field: SerializeField, Range(0, .12f)] public float CameraShakeIntensity { get; private set; } = .055f;
    [field: SerializeField, Range(1, 24)] public float CameraShakeFrequency { get; private set; } = 11f;
    [field: SerializeField, Range(0, 5)] public float MaximumPropForce { get; private set; } = 2f;
    [field: SerializeField, Range(0, 1)] public float AudioIntensity { get; private set; } = .7f;
    [field: SerializeField, Range(0, .45f)] public float LightFlickerIntensity { get; private set; } = .28f;
    [field: SerializeField, Range(0, 8)] public float MaximumDustEmission { get; private set; } = 6f;
    [field: SerializeField, Range(0, 12)] public int MaximumActivePhysicsProps { get; private set; } = 6;
    [field: SerializeField, Min(20)] public float TargetSeconds { get; private set; } = 100f;

    public float Progress(float elapsed) => Duration <= 0 ? 1f : Mathf.Clamp01(elapsed / Duration);
    public float Evaluate(float elapsed) => EvaluateCurve(IntensityCurve, elapsed);
    public float EvaluateCurve(AnimationCurve curve, float elapsed) => curve == null ? 0f : Mathf.Clamp01(curve.Evaluate(Progress(elapsed)));
    private static AnimationCurve DefaultCurve() => new(new Keyframe(0, 0), new Keyframe(.25f, .38f), new Keyframe(.55f, 1), new Keyframe(.78f, .72f), new Keyframe(1, 0));
}
