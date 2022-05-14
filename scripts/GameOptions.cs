using System;
using Godot;
using System.Text.Json;

public enum ParticleQuality
{
    Disabled,
    Low,
    Full
}


public class GameOptions
{
    // User settings system for the game
    // Instance members are the actual options, static members deal with saving/loading/general managing

    public bool MovingMusicEnabled { get; set; } = true;
    public ParticleQuality ParticleQuality { get; set; } = ParticleQuality.Full;

    public static GameOptions Current = new GameOptions();
    private static readonly string fileName = "user://gameOptions.json";

    public static void Load()
    {
        string errorPrefix = $"Failed to load game settings from {fileName}: ";

        var file = new File();
        try
        {
            if (file.Open(fileName, File.ModeFlags.Read) == Error.Ok)
            {
                Current = JsonSerializer.Deserialize<GameOptions>(file.GetAsText());
            }
            else GD.PrintErr(errorPrefix + "File not found");
        }
        catch (JsonException )
        {
            GD.PrintErr(errorPrefix + "Failed parsing JSON");
        }
        finally
        {
            file.Close();
        }
    }

    public static void Save()
    {
        using (var file = new File())
        {
            if(file.Open(fileName, File.ModeFlags.Write) == Error.Ok) file.StoreString(JsonSerializer.Serialize(Current));
            else GD.PrintErr($"Failed to save game settings to {fileName}");
        }
    }
}