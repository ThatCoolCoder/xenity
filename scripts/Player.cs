using Godot;
using System;

public class Player : KinematicBody2D
{
	private float gravity = (float) (int) ProjectSettings.GetSetting("physics/2d/default_gravity");
	public Vector2 Velocity = Vector2.Zero;
	[Export] private float maxMoveSpeed = 400;
	[Export] private float moveAcceleration = 1200;
	[Export] private float brakingAcceleration = 1200;
	[Export] private float jumpAcceleration = 600 * 7;
	[Export] private float maxJumpSpeed = 600;
	private bool isJumping = false;
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
		// Initial jump
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			Velocity.y = -jumpAcceleration * delta;
			isJumping = true;
		}
		else if (Input.IsActionPressed("jump") && isJumping)
		{
			Velocity.y -= jumpAcceleration * delta;
			if (Velocity.y < -maxJumpSpeed)
			{
				Velocity.y = -maxJumpSpeed;
				isJumping = false;
			}
		}
		else if (Input.IsActionJustReleased("jump")) isJumping = false;

		if (xAcceleration == 0)
			Velocity.x = Utils.ConvergeValue(Velocity.x, 0, brakingAcceleration * delta);
		
		Velocity.x += xAcceleration * delta;
		Velocity.x = Mathf.Clamp(Velocity.x, -maxMoveSpeed, maxMoveSpeed);

		Velocity = MoveAndSlide(Velocity, Vector2.Up);
		HandleCollisions();
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
				if (collider.IsInGroup("kills_player"))
				{
					isAlive = false;
					OnDie?.Invoke();
				}
			}
		}
	}
}
