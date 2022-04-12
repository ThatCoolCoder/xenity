using Godot;
using System;

public class Main : Node2D
{
	// Main gameplay scene, handles score and managing the other scenes

	public static float SpeedMultiplier;
	public static int Score;
	[Export] private float startingSpeedMultiplier = 1;
	[Export] private float speedMultiplierIncrement = 0.025f;
	[Export] private float maxSpeedMultiplier = 1.75f;
	private Player player;
	private PlayerChaser playerChaser;
	private Timer afterPlayerDieTimer;
	private HUD HUD;
	private Control afterDieHUD;
	
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.OnDie += OnPlayerDied;
		playerChaser = GetNode<PlayerChaser>("PlayerChaser");
		afterPlayerDieTimer = GetNode<Timer>("AfterPlayerDieTimer");
		HUD = GetNode<HUD>("CanvasLayer/HUD");
		afterDieHUD = GetNode<Control>("CanvasLayer/AfterDieHUD");

		SpeedMultiplier = startingSpeedMultiplier;
		Score = 0;
	}

	public override void _Process(float delta)
	{
		// Update score
		Score = Mathf.Max((int) (player.GlobalPosition.x / 10f), Score);

		// Increase speed
		SpeedMultiplier = Mathf.Min(SpeedMultiplier + speedMultiplierIncrement * delta, maxSpeedMultiplier);
	}

	private void OnPlayerDied()
	{
		// Pause game 
		SpeedMultiplier = 0;
		
		afterPlayerDieTimer.Start();
		HUD.Hide();
	}

	private void _on_AfterPlayerDieTimer_timeout()
	{
		afterDieHUD.Show();
	}
}
