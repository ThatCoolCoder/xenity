using Godot;
using System;

public class AfterDieHUD : Control
{
	public override void _Ready()
	{
		GetNode<Label>("VBoxContainer/ScoreLabel").Text =
			$"Score: {Main.Score}    Best: <int>";
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
