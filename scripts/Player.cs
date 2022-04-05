using Godot;
using System;

public class Player : KinematicBody2D
{
	// Player controller
	// - Left/right movement with acceleration
	// - Variable height jump
	// - Animation control

	[Export] public float SpeedMultiplier = 1;
	[Export] private float maxMoveSpeed = 400;
	[Export] private float moveAcceleration = 1200;
	[Export] private float brakingAcceleration = 1200;
	[Export] private float fullJumpDuration = 0.6f; // how long to hold jump button to get a full-height jump
	[Export] private float maxJumpSpeed = 1000;
	public Vector2 Velocity = Vector2.Zero;
	private AnimatedSprite sprite;
	private float jumpStartTime; // time at which current jump was started
	private float gravity = (float) (int) ProjectSettings.GetSetting("physics/2d/default_gravity");
	private bool isAlive = true;

	public event Action OnDie;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite>("Sprite");	
	}

	public override void _PhysicsProcess(float delta)
	{
		if (isAlive) Move(delta);
		SetAnimation();
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
			// Calculate how long the jump button has been pressed and use that to calculate
			// what size jump to do.
			float timeSinceJumpStarted = ((float) OS.GetTicksMsec() - jumpStartTime) / 1000;
			float newJumpVelocity = -maxJumpSpeed * Mathf.Min(1, timeSinceJumpStarted / fullJumpDuration);
			Velocity.y = Mathf.Max(newJumpVelocity, Velocity.y);
		}

		if (xAcceleration == 0)
			Velocity.x = Utils.ConvergeValue(Velocity.x, 0, brakingAcceleration * delta);
		
		Velocity.x += xAcceleration * delta;
		Velocity.x = Mathf.Clamp(Velocity.x, -maxMoveSpeed, maxMoveSpeed);
		
		HandleCollisions();
		Velocity = MoveAndSlide(Velocity * SpeedMultiplier, Vector2.Up) / SpeedMultiplier;
	}

	private void SetAnimation()
	{
		// Set the animation based on what the player is doing

		bool isMoving = Mathf.Abs(Velocity.x) > 0;

		if (isMoving) sprite.FlipH = Velocity.x < 0;

		if (IsOnFloor()) sprite.Animation = isMoving ? "move_right" : "idle";
		else sprite.Animation = Velocity.y < 0 ? "jump" : "fall";
	}

	public void Die()
	{
		isAlive = false;
		OnDie?.Invoke();
	}

	private void HandleCollisions()
	{
		for (int i = 0; i < GetSlideCount(); i ++) HandleCollision(GetSlideCollision(i));
	}

	private void HandleCollision(KinematicCollision2D collision)
	{
		if (typeof(StaticBody2D).IsAssignableFrom(collision.Collider.GetType()))
		{
			var collider = (StaticBody2D) collision.Collider;
			if (collider.IsInGroup("kills_player")) Die();
		}
	}

	private void _on_VisibilityNotifier2D_screen_exited()
	{
		Die();
	}
}
