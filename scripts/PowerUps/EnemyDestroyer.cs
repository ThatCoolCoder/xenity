using System;
using Godot;

public class EnemyDestroyer : TimedPowerUp
{
	// Power up that destroys all enemies for a certain length of time

	protected new bool deleteOnDeactivate = true;
	
	protected override void WhileActivated(float delta)
	{
		var enemies = GetTree().GetNodesInGroup("kills_player");
		foreach (var enemy in enemies)
		{
			if (enemy is BaseEnemy)
			{
				((BaseEnemy) enemy).Crumble();
			}
		}
	}
}
