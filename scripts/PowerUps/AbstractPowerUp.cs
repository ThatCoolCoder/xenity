using System;
using Godot;

public class AbstractPowerUp : Area2D
{
	// Abstract class representing an object in the world that can be picked up and
	// help the player some way

	[Export] private PackedScene pickUpEffect; // effect played when picked up
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

				if (pickUpEffect != null)
				{
					var instance = pickUpEffect.Instance<Node2D>();
					instance.Position = Position;
					GetParent().AddChild(instance);
				}
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