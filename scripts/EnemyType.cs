using System;
using Godot;

public struct EnemyType
{
    public float MinScale;
    public float MaxScale;
    public PackedScene PackedScene;
    public Vector2 PositionOffset; // Scaled with enemy, should be a pixel value of when scale is 1
}