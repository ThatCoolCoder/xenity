using System;
using Godot;

public class AbstractPowerUp : Area2D
{
    private bool activated;
    protected virtual bool deleteOnDeactivate = true;

    private void _on_BasePowerUp_body_entered(object body)
    {
        if (typeof(KinematicBody2D).IsAssignableFrom(body))
		{
			var object = (KinematicBody2D) collider;
            if (object.is_in_group("player"))
            {
                activated = true;
                OnActivated();
            }
		}
    }

    public void _PhysicsProcess(float delta)
    {
        if (activated) WhileActivated(delta);
    }

    public void Deactivate()
    {
        activated = false;
        OnDeactivated();
        if (deleteOnDeactivate) QueueFree();
    }


    // Called when power up is first activatedf
    protected virtual void OnActivated() {};
    protected virtual void WhileActivated(float delta) {};
    protected virtual void OnDeactivated() {};
}	