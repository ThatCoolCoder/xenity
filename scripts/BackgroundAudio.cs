using System;
using Godot;
using System.Collections.Generic;

class BackgroundAudio : ResumingAudio
{
    // Class for the background sound of the game.
    // Oscillates from left to right speaker in time with the song

    [Export] private float oscillationFrequency = 8;
    [Export] private float oscillationAmplitude = 512;
    [Export] private float oscillationOffset = 0;

    // Use a static dictionary so that we can persist positions across loading scenes 
    private static Dictionary<string, float> sineInputs = new Dictionary<string, float>();

    public override void _Process(float delta)
    {
        if (GameOptions.Current.MovingMusicEnabled)
        {
            if (! sineInputs.ContainsKey(profileName)) sineInputs[profileName] = 0;
            sineInputs[profileName] += delta;

            Position = new Vector2(
                Mathf.Sin(sineInputs[profileName] * Mathf.Tau / oscillationFrequency) * oscillationAmplitude + oscillationOffset,
                Position.y);
        }
        else
        {
            Position = new Vector2(oscillationAmplitude, 0);
        }
    }

}