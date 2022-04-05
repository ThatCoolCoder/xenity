using Godot;
using System;

public class StartScreen : Node2D
{
	private void _on_Button_pressed()
	{
		GetTree().ChangeScene("res://scenes/Main.tscn");
	}
}
