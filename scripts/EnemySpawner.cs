using Godot;
using System;
using System.Collections.Generic;

public class EnemySpawner : Node2D
{
	// Predicts player's future position and spawns traps/enemies there

	[Export] public bool Enabled = true;
	[Export] private float groundEnemyProbability = 1f / 40f;
	[Export] private float airEnemyProbability = 0; // todo: make proper air enemies
	[Export] private float predictionDistance = 0.5f; // predict position this many seconds in future

	private float rayCastVerticalOffset = -2000;
	private float gravity = (float) (int) ProjectSettings.GetSetting("physics/2d/default_gravity");


	private RayCast2D rayCast;
	[Export] private NodePath playerPath;
	private Player player;

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
			PackedScene = ResourceLoader.Load<PackedScene>("res://scenes/SpikeEnemy.tscn"),
			MinScale = 1,
			MaxScale = 1,
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
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Enabled) SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		// Use player position prediction to spawn enemies 

		var prediction = PredictPlayerPosition();
		var enemyType = Utils.RandomElement(prediction.IsOnGround ? groundEnemies : airEnemies);
		var enemyProbability = prediction.IsOnGround ? groundEnemyProbability : airEnemyProbability;

		if (random.NextDouble() < enemyProbability)
		{
			var enemy = (BaseEnemy) enemyType.PackedScene.Instance();
			var scale = (float) GD.RandRange(enemyType.MinScale, enemyType.MaxScale);
			enemy.Scale = Vector2.One * scale;
			enemy.GlobalPosition = prediction.Position + enemyType.PositionOffset * scale;
			AddChild(enemy);
		}
	}

	private PlayerPositionPrediction PredictPlayerPosition()
	{
		// Predict where the player will be in predictionDistance seconds

		// Basic velocity-based calculation
		Vector2 futurePosition = player.Position + (player.Velocity * predictionDistance);
		float gravityOffset = gravity * predictionDistance * predictionDistance * 0.5f;
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
