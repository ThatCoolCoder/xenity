using System;
using System.Collections.Generic;
using Godot;

public class PowerUpSpawner : Node2D
{
	[Export] public bool Enabled = true;
	[Export] private float powerUpProbability = 1f / 400f;
	[Export] private float powerUpXOffset = 200; // spawns power ups this far past right side of viewport

	// Power ups and their relative probabilities of spawning
	private Dictionary<PackedScene, float> powerUps = new Dictionary<PackedScene, float>()
	{
		{ ResourceLoader.Load<PackedScene>("res://scenes/PowerUps/EnemyDestroyer.tscn"), 1}
	};
	private RayCast2D upwardRayCast;
	private RayCast2D downwardRayCast;
	private RandomNumberGenerator random = new RandomNumberGenerator();

	public override void _Ready()
	{
		upwardRayCast = GetNode<RayCast2D>("UpwardRayCast");
		downwardRayCast = GetNode<RayCast2D>("DownwardRayCast");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (random.Randf() < powerUpProbability)
		{
			// Calculate power up position, mainly using raycast to find passage top and bottom


			var rightOfScreen = (GetViewportTransform().AffineInverse() * GetViewportRect().Size).x;

			// Cast downward
			downwardRayCast.GlobalPosition = new Vector2(rightOfScreen + powerUpXOffset, downwardRayCast.GlobalPosition.y);
			downwardRayCast.ForceRaycastUpdate();

			var passageBottom = GetViewportRect().Size.y;
			if (downwardRayCast.IsColliding()) passageBottom = downwardRayCast.GetCollisionPoint().y;

			// Cast upward
			upwardRayCast.GlobalPosition = new Vector2(rightOfScreen + powerUpXOffset, passageBottom);
			upwardRayCast.ForceRaycastUpdate();

			float passageTop = 0;
			if (upwardRayCast.IsColliding()) passageTop = upwardRayCast.GetCollisionPoint().y;

			var yPosition = random.RandfRange(passageTop, passageBottom);

			// Instantiate power up
			var powerUpScene = Utils.RandomElementWeighted(powerUps);
			var powerUp = powerUpScene.Instance<AbstractPowerUp>();

			powerUp.GlobalPosition = new Vector2(rightOfScreen + powerUpXOffset, yPosition);
			AddChild(powerUp);
		}
	}
}
