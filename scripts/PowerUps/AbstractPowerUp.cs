using System;
using Godot;

public class AbstractPowerUp : Area2D
{
	private bool activated;
	protected bool deleteOnDeactivate = true;

	private void _on_BasePowerUp_body_entered(object body)
	{
		if (typeof(KinematicBody2D).IsAssignableFrom(body.GetType()))
		{
			var obj = (KinematicBody2D) body;
			if (obj.IsInGroup("player"))
			{
				activated = true;
				OnActivated();
				Visible = false;
			}
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (activated) WhileActivated(delta);
	}

	public void Deactivate()
	{
		activated = false;
		OnDeactivated();
		if (deleteOnDeactivate) QueueFree();
	}


	// Called when power up is first activated
	protected virtual void OnActivated() {}
	protected virtual void WhileActivated(float delta) {}
	protected virtual void OnDeactivated() {}
}
