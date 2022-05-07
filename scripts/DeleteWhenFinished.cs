using System;
using System.Linq;
using System.Collections.Generic;
using Godot;

public class DeleteWhenFinished : Node2D
{
	// Node that deletes itself when all sub-items are finished. Useful for cleaning up effects.
	// Assumes items autostart or start in _Ready
	// Supported types for items: Particles2D, CPUParticles2D, AudioStreamPlayer, AudioStreamPlayer2D

	[Export] private List<NodePath> nodePaths = new List<NodePath>();
	private List<Node> nodes = new List<Node>();

	public override void _Ready()
	{
		foreach (var nodePath in nodePaths)
		{
			nodes.Add(GetNode(nodePath));
		}
	}

	public override void _Process(float delta)
	{
		var anyPlaying = nodes.Any(x => {
			switch (x)
			{
				case CPUParticles2D particles:
					return particles.Emitting;
				case Particles2D particles:
					return particles.Emitting;
				case AudioStreamPlayer player:
					return player.Playing;
				case AudioStreamPlayer2D player:
					return player.Playing;
				default:
					GD.PrintErr($"Unexpected type: {x.GetType()}");
					return false;
			}
		});
		if (! anyPlaying) QueueFree();
	}

}
