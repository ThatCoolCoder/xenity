using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export] public float SpeedMultiplier = 1;
	[Export] private float maxMoveSpeed = 400;
	[Export] private float moveAcceleration = 1200;
	[Export] private float brakingAcceleration = 1200;
	[Export] private float fullJumpDuration = 0.4f;
	[Export] private float maxJumpSpeed = 1000;
	public Vector2 Velocity = Vector2.Zero;
	private float gravity = (float) (int) ProjectSettings.GetSetting("physics/2d/default_gravity");
	private float jumpStartTime;
	private bool isAlive = true;

	public event Action OnDie;

	public override void _PhysicsProcess(float delta)
	{
		if (isAlive) Move(delta);
	}
	private void Move(float delta)
	{
		Velocity.y += gravity * delta;
		float xAcceleration = 0;
		if (Input.IsActionPressed("move_right")) xAcceleration += moveAcceleration;
		if (Input.IsActionPressed("move_left")) xAcceleration -= moveAcceleration;
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			Velocity.y = -maxJumpSpeed;
			jumpStartTime = OS.GetTicksMsec();
		}
		else if (Input.IsActionJustReleased("jump"))
		{
			float timeSinceJump = ((float) OS.GetTicksMsec() - jumpStartTime) / 1000;
			float newJumpVelocity = -maxJumpSpeed * Mathf.Min(1, timeSinceJump / fullJumpDuration);
			Velocity.y = Mathf.Max(newJumpVelocity, Velocity.y);
		}

		if (xAcceleration == 0)
			Velocity.x = Utils.ConvergeValue(Velocity.x, 0, brakingAcceleration * delta);
		
		Velocity.x += xAcceleration * delta;
		Velocity.x = Mathf.Clamp(Velocity.x, -maxMoveSpeed, maxMoveSpeed);
		
		HandleCollisions();
		Velocity = MoveAndSlide(Velocity * SpeedMultiplier, Vector2.Up) / SpeedMultiplier;
	}

	public void Die()
	{
		isAlive = false;
		OnDie?.Invoke();
	}

	private void HandleCollisions()
	{
		// todo: make player die when hitting stuff
		for (int i = 0; i < GetSlideCount(); i ++)
		{
			var collision = GetSlideCollision(i);
			if (typeof(StaticBody2D).IsAssignableFrom(collision.Collider.GetType()))
			{
				var collider = (StaticBody2D) collision.Collider;
				if (collider.IsInGroup("kills_player")) Die();
			}
		}
	}

	private void _on_VisibilityNotifier2D_screen_exited()
	{
		Die();
	}
}
