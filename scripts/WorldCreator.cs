using Godot;
using System;
using System.Collections.Generic;

public class WorldCreator : Node2D
{
	private float rightBufferSize = 1000; // build world slabs if the last one is closer than this to right of screen

	[Export] private bool enabled = true;
	[Export] private float startPosition;

	// Cannot fill in a thing containing self-methods at initialization time, so do it in _Ready()
	private Dictionary<Action, float> builders = new Dictionary<Action, float>();
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
		builders = new Dictionary<Action, float>()
		{
			{CreateWideSection, 3.0f},
			{CreateGapSection,  1.0f},
			{CreateNarrowSection, 0.7f},
			{CreateDownwardShaftSection, 0.7f}
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
			Utils.RandomElementWeighted(builders)();
		}
	}

	private void AddWorldSlab(PackedScene slabType, Vector2 position, Vector2 size)
	{
		var slab = (WorldSlab) slabType.Instance();
		slab.Size = size;
		slab.GlobalPosition = position;
		GetParent().AddChild(slab);
	}
	
	private float ClampFloorHeight(float floorHeight) => ClampFloorHeight(floorHeight, WorldLimitations.MinRoomHeight);

	private float ClampFloorHeight(float floorHeight, float minRoomHeight)
	{
		return Mathf.Clamp(floorHeight,
			WorldLimitations.MinCeilingPos + minRoomHeight,
			WorldLimitations.MaxFloorPos);	
	}

	private void CreateWideSection()
	{
		var length = (float) GD.RandRange(200, 350);
		var floorHeight = ClampFloorHeight(lastFloorHeight + (float) GD.RandRange(-150, 300));
		
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
		var gapLength = (float) GD.RandRange(100, 350);
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
	
	private void CreateNarrowSection()
	{
		var length = (float) GD.RandRange(100, 200);
		float height = 100;
		float endGapLength = 150;
		
		AddWorldSlab(groundSlabScene,
			new Vector2(lastWorldSlabEnd, lastFloorHeight),
			new Vector2(length + endGapLength, WorldLimitations.FloorSlabHeight));
		AddWorldSlab(ceilingSlabScene,
			new Vector2(lastWorldSlabEnd, 0),
			new Vector2(length, lastFloorHeight - height));
		AddWorldSlab(ceilingSlabScene,
			new Vector2(lastWorldSlabEnd + length, 0),
			new Vector2(endGapLength, lastFloorHeight - WorldLimitations.MinRoomHeight));
		lastWorldSlabEnd += length + endGapLength;
	}
	
	private void CreateDownwardShaftSection()
	{
		float length = 100;
		float endGapLength = 50;
		float endGapHeight = WorldLimitations.MinRoomHeight;
		
		AddWorldSlab(groundSlabScene,
			new Vector2(lastWorldSlabEnd, WorldLimitations.MaxFloorPos),
			new Vector2(length + endGapLength, WorldLimitations.FloorSlabHeight));
		AddWorldSlab(ceilingSlabScene,
			new Vector2(lastWorldSlabEnd, 0),
			new Vector2(length, WorldLimitations.MinCeilingPos));
		AddWorldSlab(ceilingSlabScene,
			new Vector2(lastWorldSlabEnd + length, 0),
			new Vector2(endGapLength, WorldLimitations.MaxFloorPos - endGapHeight));
		
		lastWorldSlabEnd += length + endGapLength;
		lastFloorHeight = WorldLimitations.MaxFloorPos;
	}
}
