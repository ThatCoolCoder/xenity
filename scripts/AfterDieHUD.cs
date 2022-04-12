using Godot;
using System;

public class AfterDieHUD : Control
{
	private void _on_AfterDieHUD_visibility_changed()
	{
		// Setup text etc when shown


		if (Visible)
		{
			int highScore = HighScoreManager.LoadHighScore();

			// If we set a new high score, say that
			if (highScore == Main.Score)
			{
				GetNode<Label>("VBoxContainer/ScoreLabel").Text =
					$"Score: {Main.Score} - New Best!";
			}
			// Otherwise say current score and what the high is
			else
			{
				GetNode<Label>("VBoxContainer/ScoreLabel").Text =
					$"Score: {Main.Score}    Best: {highScore}";
			}
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
