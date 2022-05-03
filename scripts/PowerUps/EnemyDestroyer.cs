using System;
using Godot;

public class EnemyDwstroyer : AbstractPowerUp
{
	[Export] static float test;

	float duration = 1.0f;
	
    protected override void WhileActivated(float delta)
    {
        var enemies = GetTree().GetNodesInGroup("kills_player");
        foreach (var enemy of enemies)
        [
            if (typeof(BaseEnemy).IsAssignableFrom(enemy))
            {
                ((BaseEnemy) enemy).Crumble();
            }
        ]
    }
}
