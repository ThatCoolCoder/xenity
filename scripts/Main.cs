using Godot;
using System;

public class Main : Node2D
{
	public static float SpeedMultiplier;
	[Export] private float startingSpeedMultiplier = 1;
	[Export] private float speedMultiplierIncrement = 0.025f;
	[Export] private float maxSpeedMultiplier = 1.75f;

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

		SpeedMultiplier = startingSpeedMultiplier;
	}

	public override void _Process(float delta)
	{
		// Update score
		score = Mathf.Max((int) (player.GlobalPosition.x / 10f), score);
		HUD.Score = score;

		// Increase speed
		SpeedMultiplier = Mathf.Min(SpeedMultiplier + speedMultiplierIncrement * delta, maxSpeedMultiplier);
	}

	private void OnPlayerDied()
	{
		GetTree().ReloadCurrentScene();
	}
}
