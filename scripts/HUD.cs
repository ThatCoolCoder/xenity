using Godot;
using System;

public class HUD : Control
{
	// Heads-up-display for in-game UI like score

	private Label scoreLabel;
	private Label highScoreLabel;
	private int highScore;

	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("VBoxContainer/ScoreLabel");
		highScoreLabel = GetNode<Label>("VBoxContainer/HighScoreLabel");
		highScore = HighScoreManager.LoadHighScore();
	}

	public override void _Process(float delta)
	{
		scoreLabel.Text = $"Score: {Main.Score}";
		highScoreLabel.Text = $"Best: {highScore}";
	}
}
