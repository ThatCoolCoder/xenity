using System;
using Godot;

public class EnemyDestroyer : TimedPowerUp
{
	protected new bool deleteOnDeactivate = true;
	
	protected override void WhileActivated(float delta)
	{
		var enemies = GetTree().GetNodesInGroup("kills_player");
		foreach (var enemy in enemies)
		{
			if (typeof(BaseEnemy).IsAssignableFrom(enemy.GetType()))
			{
				((BaseEnemy) enemy).Crumble();
			}
		}
	}
}
