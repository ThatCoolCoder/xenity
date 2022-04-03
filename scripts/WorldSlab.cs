using Godot;
using System;

public class WorldSlab : StaticBody2D
{
	[Export] public Vector2 Size = new Vector2(20, 20);

	private Sprite sprite;
	private CollisionShape2D collider;
	private VisibilityNotifier2D visibilityNotifier;


	public override void _Ready()
	{
		sprite = GetNode<Sprite>("Sprite");
		collider = GetNode<CollisionShape2D>("CollisionShape2D");
		visibilityNotifier = GetNode<VisibilityNotifier2D>("VisibilityNotifier2D");
	}

	public override void _Process(float delta)
	{
		sprite.RegionRect = new Rect2(sprite.RegionRect.Position, Size / 2);

		var colliderRect = new RectangleShape2D();
		colliderRect.Extents = Size / 2;
		collider.Position = Size / 2;
		collider.Shape = colliderRect;

		visibilityNotifier.Rect = new Rect2(visibilityNotifier.Rect.Position, Size / 2);

	}

	private void _on_VisibilityNotifier2D_screen_exited()
	{
		// QueueFree();
	}

}

