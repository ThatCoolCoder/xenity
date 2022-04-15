using Godot;
using System;

public class Instructions : Node2D
{
	private string showInstructionsFilePath = "user://instructionsVisible.dat";
	public event Action ReadyToStart;

	public override void _Ready()
	{
		// If file says to hide, then do that and also start game in that case
		
		bool showInstructions = true;
		using (var file = new File())
		{
			if (file.FileExists(showInstructionsFilePath))
			{
				file.Open(showInstructionsFilePath, File.ModeFlags.Read);
				showInstructions = file.Get8() != 0;
			}
		}

		if (! showInstructions)
		{

			Hide();
			// Have to call after Ready so that ReadyToStart will have been subscribed to by other nodes
			CallDeferred("Start");
		}
	}

	private void Start()
	{
		ReadyToStart?.Invoke();
	}

	public override void _Process(float delta)
	{
		if (Visible && Input.IsActionJustPressed("hide_instructions"))
		{
			// Save that the instructions should be hidden
			var file = new File();
			try
			{
				file.Open(showInstructionsFilePath, File.ModeFlags.Write);
				file.Store8(0);
			}
			catch
			{
				GD.PushWarning($"Failed writing to {showInstructionsFilePath}");
			}
			finally
			{
				file.Close();
			}

			// Hide self and start game
			Hide();
			ReadyToStart?.Invoke();
		}
	}
}
