using Godot;
using System;

public class PlayerDieEffect : Node2D
{
	// Why cannot particles autostart in godot?

	private CPUParticles2D particles1;
	private CPUParticles2D particles2;
	
	public override void _Ready()
	{
		GetNode<CPUParticles2D>("Particles1").Emitting = true;
		GetNode<CPUParticles2D>("Particles2").Emitting = true;
	}
}
