using Godot;
using System;

public class BaseEnemy : StaticBody2D
{
	// Basic enemy behaviour, handles growing/appearing and deleting when off screen

	private VisibilityNotifier2D visibilityNotifier;
	private AnimatedSprite sprite;
	private CollisionShape2D collisionShape2D;

	public bool growing = true;
	public bool crumbling = false;

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

	public virtual void Crumble()
	{
		// Make the enemy crumble and become ineffective. Used by power ups
		if (! crumbling)
		{
			sprite.Animation = "crumbling";
			crumbling = true;
			growing = false;
		}
	}

	private void _on_Sprite_animation_finished()
	{
		if (growing)
		{
			sprite.Animation = "fullyGrown";
			growing = false;
			collisionShape2D.Disabled = false;
		}
		else if (crumbling)
		{
			QueueFree();
		}
	}
}
