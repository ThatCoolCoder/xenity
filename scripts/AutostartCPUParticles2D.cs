using Godot;

public class AutostartCPUParticles2D : VariableQualityParticles
{
	// Why is this not inbuilt into godot????

	public override void _Ready()
	{
		Emitting = true;
		base._Ready();
	}
}
