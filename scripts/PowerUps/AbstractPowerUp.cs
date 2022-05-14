using System;
using Godot;

public class AbstractPowerUp : Area2D
{
	// Abstract class representing an object in the world that can be picked up and then helps the player in some way

	[Export] private PackedScene pickUpEffect; // effect played when picked up
	private bool activated;
	protected bool deleteOnDeactivate = true;

	private void _on_BasePowerUp_body_entered(object body)
	{
		// Activate when picked up

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

	// Override the following methods to add behaviour to derived classes

	// Called when power up is first activated
	protected virtual void OnActivated() {}
	// Called every physics frame that the power up is activated
	protected virtual void WhileActivated(float delta) {}
	// Called when power up is deactivated
	protected virtual void OnDeactivated() {}
}
