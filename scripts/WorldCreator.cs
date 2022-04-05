using Godot;
using System;
using System.Collections.Generic;

public class WorldCreator : Node2D
{
	// Class for procedural generation of world as player moves to the right 

	[Export] private bool enabled = true;
	[Export] private float startPosition; // Generate world only after this position

	// Dictionary of world section builder to relative probability of it being chosen,
	// values set in _Ready()
	private Dictionary<Action, float> builders = new Dictionary<Action, float>();
	private float lastWorldSlabEnd;
	private float lastFloorHeight = 300;
	private float lastCeilingHeight = 0;
	private float rightBufferSize = 1000; // build slabs this many pixels past the right of the screen

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
		// If we are running out of world, create some more

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
		AddChild(slab);
	}
	
	private float ClampFloorHeight(float floorHeight) => ClampFloorHeight(floorHeight, WorldLimitations.MinRoomHeight);

	private float ClampFloorHeight(float floorHeight, float minRoomHeight)
	{
		// Clamp the floor height so that it won't make the room too short
		// and won't make the room go above the top of the screen
		return Mathf.Clamp(floorHeight,
			WorldLimitations.MinCeilingPos + minRoomHeight,
			WorldLimitations.MaxFloorPos);	
	}

	private void CreateWideSection()
	{
		// Create a section of the world looking like so:
		// -----
		//
		// _____
		// 


		var length = (float) GD.RandRange(200, 350);
		var floorHeight = lastFloorHeight + (float) GD.RandRange(-150, 300);
		floorHeight = Mathf.Max(lastCeilingHeight + WorldLimitations.MinRoomHeight,
			floorHeight);
		floorHeight = ClampFloorHeight(floorHeight);
		
		AddWorldSlab(groundSlabScene,
			new Vector2(lastWorldSlabEnd, floorHeight),
			new Vector2(length, WorldLimitations.FloorSlabHeight));
		AddWorldSlab(ceilingSlabScene,
			new Vector2(lastWorldSlabEnd, 0),
			new Vector2(length, WorldLimitations.MinCeilingPos));

		lastWorldSlabEnd += length;
		lastFloorHeight = floorHeight;
		lastCeilingHeight = WorldLimitations.MinCeilingPos;
	}

	private void CreateGapSection()
	{
		// Create a section of the world looking like so:
		// __|       |___
		//  
		// ___     .-----
		//    |    |

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
		lastCeilingHeight = 0;
		lastWorldSlabEnd += startLength + gapLength + endLength;
	}
	
	private void CreateNarrowSection()
	{
		// Create a section of the world looking like so:
		// -.  .-
		//  |  |
		//  |__|
		// ______


		var length = (float) GD.RandRange(100, 200);
		float height = 100;
		
		AddWorldSlab(groundSlabScene,
			new Vector2(lastWorldSlabEnd, lastFloorHeight),
			new Vector2(length, WorldLimitations.FloorSlabHeight));
		AddWorldSlab(ceilingSlabScene,
			new Vector2(lastWorldSlabEnd, 0),
			new Vector2(length, lastFloorHeight - height));
		lastWorldSlabEnd += length;
		lastCeilingHeight = lastFloorHeight - height;
	}
	
	private void CreateDownwardShaftSection()
	{
		// Create a section of the world looking like so:
		// ------.
		// 		 |
		// ___	 |--
		//   |
		//   |______

		float minDropHeight = 100;
		float length = 100;
		float endGapLength = 50;
		float endGapHeight = WorldLimitations.MinRoomHeight;

		// If the floor position is already low, downward shafts will just look weird,
		// so create a normal section in that circumstance
		if (lastFloorHeight + minDropHeight > WorldLimitations.MinRoomHeight)
		{
			CreateWideSection();
			return;
		}
		
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
		lastCeilingHeight = WorldLimitations.MaxFloorPos + endGapHeight;
	}
}
