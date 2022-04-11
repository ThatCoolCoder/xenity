using Godot;
using System;

public class HUD : Control
{
	// Heads-up-display for in-game UI like score

	public int Score;

	private Label scoreLabel;

	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScoreLabel");
	}

	public override void _Process(float delta)
	{
		scoreLabel.Text = $"Score: {Score}";
	}
}
