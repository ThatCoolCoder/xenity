using Godot;
using System;
using System.Collections.Generic;

public class WorldCreator : Node2D
{
	private float rightBufferSize = 1000; // build world slabs if the last one is closer than this to right of screen

	[Export] private bool enabled = true;
	[Export] private float startPosition;

	// Cannot fill in a list<Action> at initialization time, so do it in _Ready()
	private List<Action> builders = new List<Action>();
	private float lastWorldSlabEnd;
	private float lastFloorHeight = 300;

	private PackedScene groundSlabScene = ResourceLoader.Load<PackedScene>("res://scenes/GroundSlab.tscn");
	private PackedScene ceilingSlabScene = ResourceLoader.Load<PackedScene>("res://scenes/CeilingSlab.tscn");

	private static class WorldLimitations
	{
		public static float MinCeilingPos = 100;
		public static float MaxFloorPos = 500;
		public static float MinRoomHeight = 150;
		public static float FloorSlabHeight = 1000;
	}

	public override void _Ready()
	{
		lastWorldSlabEnd = startPosition;
		builders = new List<Action>()
		{
			CreateWideSection,
			CreateGapSection
		};
	}

	public override void _Process(float delta)
	{
		if (enabled) CreateWorld();
	}

	private void CreateWorld()
	{
		var rightOfScreen = (GetViewportTransform().AffineInverse() * GetViewportRect().Size).x;
		while (lastWorldSlabEnd < rightOfScreen + rightBufferSize)
		{
			Utils.RandomFromList(builders)();
		}
	}

	private void AddWorldSlab(PackedScene slabType, Vector2 position, Vector2 size)
	{
		var slab = (WorldSlab) slabType.Instance();
		slab.Size = size;
		slab.GlobalPosition = position;
		GetParent().AddChild(slab);
	}

	private float ClampFloorHeight(float floorHeight)
	{
		return Mathf.Clamp(floorHeight,
			WorldLimitations.MinCeilingPos + WorldLimitations.MinRoomHeight,
			WorldLimitations.MaxFloorPos);	
	}

	private void CreateWideSection()
	{
		var length = (float) GD.RandRange(200, 400);
		var floorHeight = ClampFloorHeight(lastFloorHeight + (float) GD.RandRange(-400, 150));
		
		AddWorldSlab(groundSlabScene,
			new Vector2(lastWorldSlabEnd, floorHeight),
			new Vector2(length, WorldLimitations.FloorSlabHeight));
		AddWorldSlab(ceilingSlabScene,
			new Vector2(lastWorldSlabEnd, 0),
			new Vector2(length, WorldLimitations.MinCeilingPos));

		lastWorldSlabEnd += length;
		lastFloorHeight = floorHeight;
	}

	private void CreateGapSection()
	{
		var gapLength = (float) GD.RandRange(100, 450);
		float startLength = 100;
		float endLength = 100;
		float newFloorHeight = ClampFloorHeight(lastFloorHeight + (float) GD.RandRange(-200, 100));

		AddWorldSlab(groundSlabScene,
			new Vector2(lastWorldSlabEnd, lastFloorHeight),
			new Vector2(startLength, WorldLimitations.FloorSlabHeight));
		
		AddWorldSlab(groundSlabScene,
			new Vector2(lastWorldSlabEnd + startLength + gapLength, newFloorHeight),
			new Vector2(endLength, WorldLimitations.FloorSlabHeight));

		lastFloorHeight = newFloorHeight;
		lastWorldSlabEnd += startLength + gapLength + endLength;
	}
}
