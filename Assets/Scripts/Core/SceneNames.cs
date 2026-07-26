public enum GameScene
{
    MainMenu,
    Level01,
    Level02,
    Level03,
    Simulation
}

public static class SceneNames
{
    public const string MainMenu = "MainMenu";
    public const string Level01 = "Level01";
    public const string Level02 = "Level02";
    public const string Level03 = "Level03";

    // Temporary compatibility name for the current prototype scene.
    public const string Simulation = "Simulation";

    public static string GetName(GameScene scene)
    {
        return scene switch
        {
            GameScene.MainMenu => MainMenu,
            GameScene.Level01 => Level01,
            GameScene.Level02 => Level02,
            GameScene.Level03 => Level03,
            GameScene.Simulation => Simulation,
            _ => string.Empty
        };
    }
}
