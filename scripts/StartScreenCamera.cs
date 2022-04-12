using Godot;
using System;

public class StartScreenCamera : Camera2D
{
	// Script to move the camera on the start screen

	[Export] private float speed = 0;
	[Export] private float acceleration = 100;
	[Export] private float maxSpeed = 200;

	public override void _Process(float delta)
	{
		speed += acceleration * delta;
		speed = Mathf.Min(speed, maxSpeed);
		Position = new Vector2(Position.x + speed * delta, Position.y);
	}
}
