using Godot;
using System;

public class Player : KinematicBody2D
{
	private float gravity = (float) (int) ProjectSettings.GetSetting("physics/2d/default_gravity");
	public Vector2 Velocity = Vector2.Zero;
	[Export] private float maxMoveSpeed = 400;
	[Export] private float moveAcceleration = 800;
	[Export] private float brakingAcceleration = 800;
	[Export] private float jumpSpeed = 600;

	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(float delta)
	{
		Velocity.y += gravity * delta;

		float xAcceleration = 0;
		if (Input.IsActionPressed("move_right")) xAcceleration += moveAcceleration;
		if (Input.IsActionPressed("move_left")) xAcceleration -= moveAcceleration;
		if (Input.IsActionPressed("jump") && IsOnFloor()) Velocity.y = -jumpSpeed;

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
					GD.Print("Ouch");
			}
		}
	}
}
