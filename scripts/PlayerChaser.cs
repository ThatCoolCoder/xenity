using Godot;
using System;

public class PlayerChaser : Node2D
{
	// Thing that chases the player to give them a reason to be fast.
	// Can attach player-killers, camera, etc to it

	[Export] private float speed = 0;
	[Export] private float acceleration = 100;
	[Export] private float maxSpeed = 200;
	[Export] private NodePath playerPath;
	private Player player;
	
	
	public override void _Ready()
	{
		player = GetNode<Player>(playerPath);
	}

	public override void _PhysicsProcess(float delta)
	{
		// Move forward
		speed = Mathf.Min(maxSpeed, speed + acceleration * delta * Main.SpeedMultiplier);
		float movement = speed * delta * Main.SpeedMultiplier;
		float newX = Mathf.Max(player.Position.x, Position.x + movement);
		Position = new Vector2(newX, Position.y);
	}
}
