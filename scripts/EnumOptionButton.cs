using Godot;
using System;

public class EnumOptionButton<TItem> : OptionButton where TItem : Enum
{
	// Option button (dropdown) allowing you to select a value of an enum
	// Generic classes cannot be bound to an object in Godot, so to use, create a derived class for attaching to object:
	// class MyTypeSelector : EnumOptionButton<MyEnumType>


	public override void _Ready()
	{
		GD.Print("Hello");
		foreach(TItem value in Enum.GetValues(typeof(TItem)))
		{
			AddItem(value.ToString());
		}
	}
}
