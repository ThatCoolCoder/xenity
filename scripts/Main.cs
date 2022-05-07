using Godot;
using System;

public class Main : Node2D
{
	// Main gameplay scene, handles score and managing the other scenes

	public static float SpeedMultiplier = 0;
	public static int Score;
	[Export] private float startingSpeedMultiplier = 1;
	[Export] private float speedMultiplierIncrement = 0.025f;
	[Export] private float maxSpeedMultiplier = 1.75f;
	private bool gameStarted = false;
	private Player player;
	private Position2D startPosition;
	private PlayerChaser playerChaser;
	private EnemySpawner enemySpawner;
	private PowerUpSpawner powerUpSpawner;
	private Instructions instructions;
	private Timer afterPlayerDieTimer;
	private HUD HUD;
	private Control afterDieHUD;
	
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.OnDie += OnPlayerDied;
		startPosition = GetNode<Position2D>("StartPosition");
		player.GlobalPosition = startPosition.GlobalPosition;
		
		playerChaser = GetNode<PlayerChaser>("PlayerChaser");
		enemySpawner = GetNode<EnemySpawner>("EnemySpawner");
		powerUpSpawner = GetNode<PowerUpSpawner>("PowerUpSpawner");
		instructions = GetNode<Instructions>("Instructions");
		instructions.ReadyToStart += StartGame;
		afterPlayerDieTimer = GetNode<Timer>("AfterPlayerDieTimer");
		HUD = GetNode<HUD>("CanvasLayer/HUD");
		afterDieHUD = GetNode<Control>("CanvasLayer/AfterDieHUD");

		Score = 0;
	}


	public override void _Process(float delta)
	{
		if (gameStarted)
		{
			// Update score
			float distanceTravelled = player.GlobalPosition.x - startPosition.GlobalPosition.x;
			Score = Mathf.Max((int) (distanceTravelled / 10f), Score);
			
			if (Input.IsActionJustPressed("restart")) GetTree().ReloadCurrentScene();

			// Increase speed
			if (player.isAlive) SpeedMultiplier = Mathf.Min(SpeedMultiplier + speedMultiplierIncrement * delta, maxSpeedMultiplier);
		}
	}

	private void StartGame()
	{
		gameStarted = true;
		enemySpawner.Enabled = true;
		powerUpSpawner.Enabled = true;
		SpeedMultiplier = startingSpeedMultiplier;
	}

	private void OnPlayerDied()
	{
		// Pause game 
		SpeedMultiplier = 0;
		powerUpSpawner.Enabled = false;
		
		if (afterPlayerDieTimer.IsInsideTree()) afterPlayerDieTimer.Start();
		if (HighScoreManager.LoadHighScore() < Score)
		{
			HighScoreManager.SaveHighScore(Score);
		}
	}

	private void _on_AfterPlayerDieTimer_timeout()
	{
		HUD.Hide();
		afterDieHUD.Show();
	}
}
