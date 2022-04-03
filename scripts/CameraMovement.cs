using Godot;
using System;

public class CameraMovement : Camera2D
{
	[Export] private NodePath playerPath;
	private Player player;

	public override void _Ready()
	{
		player = GetNode<Player>(playerPath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		Position = new Vector2(player.Position.x, Position.y);
	}
}
