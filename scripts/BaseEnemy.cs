using Godot;
using System;

public class BaseEnemy : StaticBody2D
{
	// Basic enemy behaviour, handles growing/appearing and deleting when off screen

	[Export] private PackedScene CrumbleEffect;

	private VisibilityNotifier2D visibilityNotifier;
	private AnimatedSprite sprite;
	private CollisionShape2D collisionShape2D;

	public bool growing = true;

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
		if (CrumbleEffect == null || growing) return;
		var effect = CrumbleEffect.Instance<CPUParticles2D>();
		effect.Position = Position;
		effect.Emitting = true;
		GetParent().AddChild(effect);
		growing = false;
		QueueFree();
	}

	private void _on_Sprite_animation_finished()
	{
		if (growing)
		{
			sprite.Animation = "fullyGrown";
			growing = false;
			collisionShape2D.Disabled = false;
		}
	}
}
