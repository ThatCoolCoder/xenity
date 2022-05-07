using Godot;

public class AutostartCPUParticles2D : CPUParticles2D
{
	// Why is this not inbuilt into godot????

	public override void _Ready()
	{
		Emitting = true;
	}
}
