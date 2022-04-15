using Godot;
using System;
using System.Collections.Generic;

public class ResumingAudio : AudioStreamPlayer2D
{
	// Audio that resumes playing at the same spot even after switching scenes
	// Add an audio stream player in each scene with this as the script.
	// Recommended settings for the player are:
	// - same audio stream for each
	// - autoplay or playing on
	// - stream paused off
	
	[Export] protected string profileName = "default";

	private static Dictionary<string, float> playbackPositions = new Dictionary<string, float>();

	public override void _EnterTree()
	{
		if (playbackPositions.ContainsKey(profileName))
		{
			Play(playbackPositions[profileName]);
		}
	}

	public override void _ExitTree()
	{
		playbackPositions[profileName] = GetPlaybackPosition();
	}
}
