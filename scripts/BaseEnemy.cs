using Godot;
using System;

public class BaseEnemy : StaticBody2D
{
	[Export] private NodePath visibilityNotifierPath;
	private VisibilityNotifier2D visibilityNotifier;

	public override void _Ready()
	{
		visibilityNotifier = GetNode<VisibilityNotifier2D>(visibilityNotifierPath);
	}

	public override void _PhysicsProcess(float delta)
	{
		// Delete if offscreen
		if (! visibilityNotifier.IsOnScreen()) QueueFree();
	}
}
