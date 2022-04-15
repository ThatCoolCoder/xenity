using Godot;
using System;
using System.Collections.Generic;

public class EnemySpawner : Node2D
{
	// Predicts player's future position and spawns traps/enemies there

	[Export] public bool Enabled = true;
	[Export] private float groundEnemyProbability = 1f / 40f;
	[Export] private float airEnemyProbability = 0; // todo: make proper air enemies
	[Export] private float predictionDistance = 0.85f; // predict position this many seconds in future
	[Export] private float maxEnemyInterval = 1.5f;
	[Export] private float rayCastVerticalOffset = -2000;
	private float lastEnemySpawnTime = OS.GetTicksMsec();
	private float gravity = (float) (int) ProjectSettings.GetSetting("physics/2d/default_gravity");


	private RayCast2D rayCast;
	[Export] private NodePath playerPath;
	private Player player;
	private Timer firstEnemyTimer;
	private bool firstEnemyTimerFinished;
	private static Random random = new Random();

	private List<EnemyType> groundEnemies = new List<EnemyType>
	{
		new EnemyType()
		{
			PackedScene = ResourceLoader.Load<PackedScene>("res://scenes/SpikeEnemy.tscn"),
			MinScale = 1,
			MaxScale = 2,
			PositionOffset = new Vector2(-8, -16)
		}
	};
	private List<EnemyType> airEnemies = new List<EnemyType>
	{
		new EnemyType()
		{
			PackedScene = ResourceLoader.Load<PackedScene>("res://scenes/AirEnemy.tscn"),
			MinScale = 1.5f,
			MaxScale = 2.5f,
			PositionOffset = new Vector2(-8, -16)
		}
	};

	private struct PlayerPositionPrediction
	{
		public Vector2 Position { get; set; }
		public bool IsOnGround { get; set; }
	}

	public override void _Ready()
	{
		rayCast = GetNode<RayCast2D>("RayCast2D");
		player = GetNode<Player>(playerPath);
		firstEnemyTimer = GetNode<Timer>("FirstEnemyDelay");
	}


	private void _on_FirstEnemyTimer_timeout()
	{
		firstEnemyTimerFinished = true;
	}


	public override void _PhysicsProcess(float delta)
	{
		if (! firstEnemyTimerFinished && Enabled && firstEnemyTimer.TimeLeft == 0) firstEnemyTimer.Start();

		if (Enabled && firstEnemyTimerFinished) SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		// Use player position prediction to spawn enemies 

		var prediction = PredictPlayerPosition();
		var enemyType = Utils.RandomElement(prediction.IsOnGround ? groundEnemies : airEnemies);
		var enemyProbability = prediction.IsOnGround ? groundEnemyProbability : airEnemyProbability;

		if (random.NextDouble() < enemyProbability || lastEnemySpawnTime + maxEnemyInterval * 1000 < OS.GetTicksMsec())
		{
			var enemy = (BaseEnemy) enemyType.PackedScene.Instance();
			var scale = (float) GD.RandRange(enemyType.MinScale, enemyType.MaxScale);
			enemy.Scale = Vector2.One * scale;
			AddChild(enemy);
			enemy.GlobalPosition = prediction.Position + enemyType.PositionOffset * scale;
			lastEnemySpawnTime = OS.GetTicksMsec();
		}
	}

	private PlayerPositionPrediction PredictPlayerPosition()
	{
		// Predict where the player will be in predictionDistance seconds

		// Scale prediction distance as game speeds up
		float scaledPredictionDistance = predictionDistance / Main.SpeedMultiplier;

		// Basic velocity-based calculation
		Vector2 futurePosition = player.Position + (player.Velocity * scaledPredictionDistance);
		float gravityOffset = gravity * scaledPredictionDistance * scaledPredictionDistance * 0.5f;
		futurePosition.y += gravityOffset;

		// Moving player out of ground if they would hit it
		var rayCastPosition = futurePosition;
		rayCastPosition.y += rayCastVerticalOffset;
		rayCast.GlobalPosition = rayCastPosition;

		bool rayHitGround = false;
		rayCast.ForceRaycastUpdate();
		if (rayCast.IsColliding() && rayCast.GetCollisionPoint().y < futurePosition.y)
		{
			futurePosition = rayCast.GetCollisionPoint();
			rayHitGround = true;
		}

		return new PlayerPositionPrediction()
		{
			Position = futurePosition,
			IsOnGround = rayHitGround
		};
	}
}
