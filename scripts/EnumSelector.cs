using System;
using System.Linq;
using Godot;
using System.Collections.Generic;

public abstract class EnumSelector<TItem> : Control where TItem : Enum
{
	// UI element allowing you to select one item from a list of options, eg a quality selector.
	// Unfortunately, generic classes cannot be bound to an object in Godot, so to use, create a derived class for attaching to object:
	// class MyTypeSelector : EnumSelector<MyEnumType> {}

	// This class has a number of somewhat complicated conversions in it, mainly due to c# enums being introduced before generics.

	[Export] public Font Font;
	private List<string> itemNames = new();
	private Label label;
	public TItem Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			label.Text = selected.ToString();
			if (valueSet) OnChanged?.Invoke(); // prevent setting value when framework sets to default
			valueSet = true;
		}
	}
	private TItem selected;
	private bool valueSet = false;
	public event Action OnChanged;

	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		label.AddFontOverride("font", Font);
		itemNames = ((TItem[]) Enum.GetValues(typeof(TItem))).Select(x => x.ToString()).ToList();
		Selected = Selected; // Update label text
	}
	
	private void _on_UpArrow_pressed()
	{
		int newValue = Mathf.Max(Convert.ToInt32(Selected) - 1, 0);
		Selected = (TItem) Enum.Parse(typeof(TItem), newValue.ToString());
	}


	private void _on_DownArrow_pressed()
	{
		int newValue = Mathf.Min(Convert.ToInt32(Selected) + 1, itemNames.Count - 1);
		Selected = (TItem) Enum.Parse(typeof(TItem), newValue.ToString());
	}

}
