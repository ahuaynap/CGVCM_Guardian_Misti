using UnityEngine;

public sealed class AssetHubGeneratedMarker : MonoBehaviour
{
    [SerializeField] private string semanticId;
    public string SemanticId => semanticId;
    public void Configure(string value) => semanticId = value;
}
