using Godot;
using System;
using System.Collections.Generic;

public class AfterDieHUD : Control
{
	// Display showing after you die showning score and asking to restart/go to main menu
	// Also displays tips/hints
	
	// Would be great to [Export] this so it can be edited in editor but Godot keeps resetting it if I do that, so do through code.
	private List<string> hints = new List<string>()
	{
		"Changing direction regularly makes it harder to predict your movement",
		"Sometimes you need to briefly go backwards to evade the spikes",
		"You will survive a lot longer if you don't touch the enemies",
		"You can press R to restart quickly",
		"You can also play the game through a joystick or controller",
		"You can disable oscillating music in the options menu",
		"Getting low FPS? Try turning down the particle settings in the options menu",
		"This game isn't meant to be easy",
	};
	
	private void _on_AfterDieHUD_visibility_changed()
	{
		// Setup content when first shown

		if (Visible)
		{
			// Show score
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
