using Godot;
using System;

public class WorldSlab : StaticBody2D
{
	// Rectangular slab of ground that deletes when off screen
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

		// To make the rect the correct size, we should use Size / 2 in the line below
		// But we want a bit of time before the rects disappear so make it twice as big
		visibilityNotifier.Rect = new Rect2(visibilityNotifier.Rect.Position, Size);

	}

	private void _on_VisibilityNotifier2D_screen_exited()
	{
		QueueFree();
	}

}

