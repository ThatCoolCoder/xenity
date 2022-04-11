using Godot;
using System;

public class AfterDieHUD : Control
{
	public override void _Ready()
	{
		GetNode<Label>("VBoxContainer/ScoreLabel").Text = $"Score: {Main.Score}";
	}
}
