using Godot;
using System;
using System.Collections.Generic;

public class AfterDieHUD : Control
{
	// Display showing after you die showning score and asking to restart/go to main menu
	// Also displays tips/hints
	
	[Export] public List<string> hints = new List<string>(); 
	
	private void _on_AfterDieHUD_visibility_changed()
	{
		// Setup content when first shown

		if (Visible)
		{
			// Set score label
			int highScore = HighScoreManager.LoadHighScore();
			GetNode<Label>("VBoxContainer/ScoreLabel").Text = highScore == Main.Score ?
				$"Score: {Main.Score} - New Best!" :
				$"Score: {Main.Score}    Best: {highScore}";
			
			// Show a hint
			GetNode<Label>("VBoxContainer/HintLabel").Text = $"Tip: {Utils.RandomElement(hints)}";
		}
	}

	private void _on_RestartButton_pressed()
	{
		GetTree().ReloadCurrentScene();
	}


	private void _on_MainMenuButton_pressed()
	{
		GetTree().ChangeScene("res://scenes/StartScreen.tscn");
	}

}
