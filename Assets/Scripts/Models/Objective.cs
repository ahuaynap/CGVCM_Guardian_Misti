using UnityEngine;

[System.Serializable]
public class Objective
{
    [SerializeField] private string id;
    [SerializeField] private string description;
    public string Id => id;
    public string Description => description;
    public Objective() { }
    public Objective(string id, string description) { this.id = id; this.description = description; }
}
