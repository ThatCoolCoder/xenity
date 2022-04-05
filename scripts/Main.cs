using Godot;
using System;

public class Main : Node2D
{
	[Export] private float speedMultiplier;
	[Export] private float startingSpeedMultiplier = 1;
	[Export] private float speedMultiplierIncrement = 0.05f;
	[Export] private float maxSpeedMultiplier = 2.5f;

	private Player player;
	private PlayerChaser playerChaser;
	private HUD HUD;
	private int score;
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.OnDie += OnPlayerDied;
		playerChaser = GetNode<PlayerChaser>("PlayerChaser");
		HUD = GetNode<HUD>("HUD");

		speedMultiplier = startingSpeedMultiplier;
	}

	public override void _Process(float delta)
	{
		// Update score
		score = Mathf.Max((int) (player.GlobalPosition.x / 10f), score);
		HUD.Score = score;

		// Increase speed
		speedMultiplier = Mathf.Min(speedMultiplier + speedMultiplierIncrement * delta, maxSpeedMultiplier);
		player.SpeedMultiplier = speedMultiplier;
		playerChaser.SpeedMultiplier = speedMultiplier;
	}

	private void OnPlayerDied()
	{
		GetTree().ReloadCurrentScene();
	}
}
