using Godot;
using System;

public class StartScreen : Node2D
{
	[Export] private string moreProjectsUrl;
	private AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		GameOptions.Load();
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	private void _on_Button_pressed()
	{
		GetTree().ChangeScene("res://scenes/Main.tscn");
	}

	private void _on_LinkButton_pressed()
	{
		OS.ShellOpen(moreProjectsUrl);
	}

	private void _on_OptionsButton_pressed()
	{
		animationPlayer.Play("show_options");
	}

	private void _on_ExitOptionsButton_pressed()
	{
		animationPlayer.Play("hide_options");
		GameOptions.Save();
	}

	private void _on_CreditsButton_pressed()
	{
		animationPlayer.Play("show_credits");
	}


	private void _on_ExitCreditsButton_pressed()
	{
		animationPlayer.Play("hide_credits");
	}


	private void _on_CreditsText_meta_clicked(string meta)
	{
		// Open link when clicked in credits menu
		OS.ShellOpen(meta);
	}

}
