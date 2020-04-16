using Godot;
using System;

public class AbilityToolbarButton : Button
{
	[Export] public ToolbarButtonType ButtonType { get; set; }
	TextureRect display;

	public void Setup()
	{
		display = (TextureRect)GetNode("TextureRect");
	}

	public void UpdateVisual(Texture icon, Color color)
	{
		display.Texture = icon;
		Modulate = color
	}

}

public enum ToolbarButtonType
{
	GENERAL,
	ABILITY,
	ITEM
}
