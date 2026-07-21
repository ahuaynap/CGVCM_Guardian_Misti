public class Objective
{
    public string Id { get; }
    public string Description { get; }

    public Objective(string id, string description)
    {
        Id = id;
        Description = description;
    }
}
