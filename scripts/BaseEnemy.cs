using Godot;
using System;

public class BaseEnemy : StaticBody2D
{
	// Basic enemy behaviour, handles growing/appearing and deleting when off screen

	[Export] private PackedScene crumbleEffect;

	private VisibilityNotifier2D visibilityNotifier;
	private AnimatedSprite sprite;
	private CollisionShape2D collisionShape2D;
	private Timer growthTimer;

	public bool growing = true;

	public override void _Ready()
	{
		visibilityNotifier = GetNode<VisibilityNotifier2D>("VisibilityNotifier2D");
		sprite = GetNode<AnimatedSprite>("Sprite");
		sprite.SpeedScale *= Main.SpeedMultiplier;
		collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		growthTimer = GetNode<Timer>("GrowthTimer");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Delete if offscreen
		if (! visibilityNotifier.IsOnScreen()) QueueFree();
	}

	public virtual void Crumble()
	{
		// Make the enemy crumble and become ineffective. Used by power ups
		if (crumbleEffect == null || growing) return;

		var effect = crumbleEffect.Instance<Node2D>();
		effect.Position = Position;
		GetParent().AddChild(effect);
		growing = false;
		QueueFree();
	}

	private void _on_Sprite_animation_finished()
	{
		// Randomly, the sprite thinks it has finished animation on the first frame,
		// so have a timer to ensure a brief pause
		if (growing && growthTimer.TimeLeft == 0)
		{
			sprite.Animation = "fullyGrown";
			growing = false;
			collisionShape2D.Disabled = false;
		}
	}
}
