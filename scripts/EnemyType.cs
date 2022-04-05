using System;
using Godot;

public struct EnemyType
{
    // Struct representing information for spawning enemies

    public float MinScale;
    public float MaxScale;
    public PackedScene PackedScene;

    // Offset of enemy position from predicted position, used to account for things like offset origin.
    // Scaled with enemy, should be a pixel value of when scale is 1
    public Vector2 PositionOffset; 
}