using Godot;
using System;

public class Main : Node2D
{
	private Player player;
	private HUD HUD;
	private int score;
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.OnDie += OnPlayerDied;
		HUD = GetNode<HUD>("HUD");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		score = Mathf.Max((int) (player.GlobalPosition.x / 10f), score);
		HUD.Score = score;
	}

	private void OnPlayerDied()
	{
		GetTree().ReloadCurrentScene();
	}
}
