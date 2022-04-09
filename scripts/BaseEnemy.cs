using Godot;
using System;

public class BaseEnemy : StaticBody2D
{
	// Basic enemy behaviour, handles growing/appearing and deleting when off screen

	private VisibilityNotifier2D visibilityNotifier;
	private AnimatedSprite sprite;
	private CollisionShape2D collisionShape2D;

	public bool fullyGrown = false;

	public override void _Ready()
	{
		visibilityNotifier = GetNode<VisibilityNotifier2D>("VisibilityNotifier2D");
		sprite = GetNode<AnimatedSprite>("Sprite");
		sprite.SpeedScale *= Main.SpeedMultiplier;
		collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Delete if offscreen
		if (! visibilityNotifier.IsOnScreen()) QueueFree();
	}

	private void _on_Sprite_animation_finished()
	{
		if (! fullyGrown)
		{
			sprite.Animation = "fullyGrown";
			fullyGrown = true;
			collisionShape2D.Disabled = false;
		}
	}
}
