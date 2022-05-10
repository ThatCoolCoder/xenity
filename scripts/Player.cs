using Godot;
using System;

public class Player : KinematicBody2D
{
	// Player controller
	// - Left/right movement with acceleration
	// - Variable height jump
	// - Can jump for a short period after falling off a ledge - this reduces player frustration
	// - Fall more quickly when pressing down
	// - Sets animation

	[Export] private float maxMoveSpeed = 400;
	[Export] private float moveAcceleration = 1200;
	[Export] private float brakingAcceleration = 1200;
	[Export] private float fullJumpDuration = 0.6f; // how long to hold jump button to get a full-height jump
	[Export] private float maxJumpSpeed = 1000; // Speed of a max-height jump
	[Export] private float postFallJumpThreshold = 0.15f; // can jump if was on ground within n seconds ago
	[Export] private float dropAcceleration = 5000; // when down key is pressed, player drops at this rate
	public bool isAlive = true;
	public Vector2 Velocity = Vector2.Zero;
	private AnimatedSprite sprite;
	private float jumpStartTime; // time at which current jump was started
	private bool jumpInProgress = true;
	private float lastOnFloor; // time at which player was last on floor
	private float gravity = (float) (int) ProjectSettings.GetSetting("physics/2d/default_gravity");
	private PackedScene dieEffectPrefab = GD.Load<PackedScene>("res://scenes/PlayerDieEffect.tscn");

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
		// Keybinds and physics
		// Unfortunately is rather complex, but that's the nature of a complex player controller

		// Remember when on floor
		if (IsOnFloor())
		{
			lastOnFloor = OS.GetTicksMsec();
			jumpInProgress = false;
		}

		// Feel gravity
		Velocity.y += gravity * delta;

		// Left/right movement
		float xAcceleration = 0;
		if (Input.IsActionPressed("move_right")) xAcceleration += moveAcceleration;
		if (Input.IsActionPressed("move_left")) xAcceleration -= moveAcceleration;

		// Jump start
		if (Input.IsActionPressed("jump") && lastOnFloor + postFallJumpThreshold * 1000 > OS.GetTicksMsec() && !jumpInProgress)
		{
			Velocity.y = -maxJumpSpeed;
			jumpStartTime = OS.GetTicksMsec();
			jumpInProgress = true;
		}
		// Jump end
		else if (Input.IsActionJustReleased("jump"))
		{
			// Calculate how long the jump button has been pressed and use that to calculate what size jump to do.
			float timeSinceJumpStarted = ((float) OS.GetTicksMsec() - jumpStartTime) / 1000;
			float newJumpVelocity = -maxJumpSpeed * Mathf.Min(1, timeSinceJumpStarted / fullJumpDuration);
			Velocity.y = Mathf.Max(newJumpVelocity, Velocity.y);
		}
		// Fall downwards quickly
		if (Input.IsActionPressed("drop") && ! IsOnFloor()) Velocity.y += dropAcceleration * delta;

		// If not pressing any keys, have friction
		if (xAcceleration == 0)
			Velocity.x = Utils.ConvergeValue(Velocity.x, 0, brakingAcceleration * delta);
		
		// Integrate velocity
		Velocity.x += xAcceleration * delta * Main.SpeedMultiplier;
		Velocity.x = Mathf.Clamp(Velocity.x, -maxMoveSpeed, maxMoveSpeed);
		
		HandleCollisions();

		// Let the engine handle movement and collision (don't move if game is "paused")
		if (Main.SpeedMultiplier > 0) Velocity = MoveAndSlide(Velocity * Main.SpeedMultiplier, Vector2.Up) / Main.SpeedMultiplier;
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
		sprite.Hide();
		var dieEffect = dieEffectPrefab.Instance<PlayerDieEffect>();
		CallDeferred("add_child", dieEffect);
		OnDie?.Invoke();
	}

	private void HandleCollisions()
	{
		for (int i = 0; i < GetSlideCount(); i ++) HandleCollision(GetSlideCollision(i).Collider);
	}

	private void HandleCollision(object collider)
	{
		// Check if any of the objects that we've collided with need special treatment

		if (collider != null)
		{
			if (collider is StaticBody2D)
			{
				var colliderBody = (StaticBody2D) collider;
				if (colliderBody.IsInGroup("kills_player") && isAlive) Die();
			}
		}
	}

	private void _on_VisibilityNotifier2D_screen_exited()
	{
		// Die when off screen
		if (isAlive) Die();
	}


	private void _on_Area2D_body_entered(object body)
	{
		// Collisions from moving static bodies don't get registered by GetSlideCollision if we're still so also have an area to detect those
		HandleCollision(body);
	}

}
