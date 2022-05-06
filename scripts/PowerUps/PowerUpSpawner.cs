using System;
using System.Collections.Generic;
using Godot;

public class PowerUpSpawner : Node2D
{
	[Export] public bool Enabled = true;
	[Export] private float powerUpProbability = 1f / 300f;
	private List<PackedScene> powerUps = new List<PackedScene>()
	{
		ResourceLoader.Load<PackedScene>("res://scenes/PowerUps/EnemyDestroyer.tscn")
	};
	private RayCast2D rayCast;
	private Random random = new Random();

	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(float delta)
	{
		if ((float) random.NextDouble() < powerUpProbability)
		{
			var powerUpScene = Utils.RandomElement(powerUps);
			var powerUp = powerUpScene.Instance<AbstractPowerUp>();
		}
	}
}
