using System;
using Godot;

public class TimedPowerUp : AbstractPowerUp
{
	private Timer timer;

	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		base._Ready();
	}

	protected override void OnActivated()
	{
		timer.Start();
		base.OnActivated();
	}

	private void _on_Timer_timeout()
	{
		Deactivate();
	}
}
