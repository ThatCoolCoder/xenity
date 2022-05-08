using System.Collections.Generic;
using Godot;
public class VariableQualityParticles : CPUParticles2D
{
    // Particle effect that can change its amount of particles to improve performance on low-end computers

    // A dictionary would be a cleaner way of doing this but dictionaries don't work well
    // in the godot editor ui.
    [Export] private int lowQualityAmount;
    [Export] private int fullQualityAmount;

    public override void _Ready()
    {
        switch (GameOptions.Current.ParticleQuality)
        {
            case ParticleQuality.Disabled:
                Amount = 1; // turns out you can't completely disable particles, so almost disable them
                break;
            case ParticleQuality.Low:
                Amount = lowQualityAmount;
                break;
            case ParticleQuality.Full:
                Amount = fullQualityAmount;
                break;
        }
        base._Ready();
    }
}