using Godot;
using System;

public class StartScreen : Node2D
{
	// Start screen/title screen showing game name, credits and options
	
	[Export] private string moreProjectsUrl;
	private AnimationPlayer animationPlayer;
	private ParticleQualitySelector particleQualitySelector;
	private CheckButton movingMusicButton;
	private bool finishedSetup = false;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		movingMusicButton = GetNode<CheckButton>("CanvasLayer/Control/Options/MovingMusicCheckButton");
		particleQualitySelector = GetNode<ParticleQualitySelector>("CanvasLayer/Control/Options/ParticleQuality/Selector");

		GameOptions.Load();
		movingMusicButton.Pressed = GameOptions.Current.MovingMusicEnabled;
		particleQualitySelector.Selected = GameOptions.Current.ParticleQuality;
		particleQualitySelector.OnChanged += UpdateOptionsFromUI;

		finishedSetup = true;
	}

	private void UpdateOptionsFromUI()
	{
		// Save options

		if (finishedSetup)
		{
			GameOptions.Current.MovingMusicEnabled = movingMusicButton.Pressed;
			GameOptions.Current.ParticleQuality = particleQualitySelector.Selected;
			GameOptions.Save();
		}
	}


	private void _on_Button_pressed()
	{
		// Start game when start button clicked 

		GetTree().ChangeScene("res://scenes/Main.tscn");
	}

	private void _on_LinkButton_pressed()
	{
		// Open link when "More Projects" clicked
		
		OS.ShellOpen(moreProjectsUrl);
	}


	// Transitions between different screens.
	private void _on_OptionsButton_pressed()
	{
		animationPlayer.Play("show_options");
	}

	private void _on_ExitOptionsButton_pressed()
	{
		animationPlayer.Play("hide_options");
	}

	private void _on_CreditsButton_pressed()
	{
		animationPlayer.Play("show_credits");
	}

	private void _on_ExitCreditsButton_pressed()
	{
		animationPlayer.Play("hide_credits");
	}

	// Update this setting in real time when clicked
	private void _on_MovingMusicCheckButton_toggled(bool button_pressed) => UpdateOptionsFromUI();

	private void _on_CreditsText_meta_clicked(string meta)
	{
		// Open link when clicked in credits menu
		OS.ShellOpen(meta);
	}

}
