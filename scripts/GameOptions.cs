public enum ParticleQuality
{
    Disabled,
    Low,
    Full
}


public static class GameOptions
{
    public static bool MovingMusicEnabled { get; set; } = true;
    public static ParticleQuality ParticleQuality { get; set; } = ParticleQuality.Full;
}