using Godot;
using System;

public class PartyMemberIcon : Button
{
	public Character IconFor { get; set; }

	public bool IsUsed { get; set; } = false;
	public TextureRect Portrait { get; set; }
	public Label healthDisplay;
	public Color Color { get; set; }

	private static Color deathColor = new Color(0.3f, 0.3f, 0.3f);

	public override void _Ready()
	{
		Portrait = (TextureRect)GetNode("Portrait");
		healthDisplay = (Label)GetNode("HealthDisplay");
		IsUsed = false;
	}

	public void Reset()
	{
		IsUsed = false;
		SetHealth(-1, -1);
		SetPortrait(null, new Color());
		Color = new Color();
	}

	public void Highlight(bool isSelected)
	{
		if (isSelected)
			Portrait.Modulate = this.Color.Lightened(.5f);
		else
			Portrait.Modulate = this.Color;
	}

	public void SetPortrait(Texture image, Color color)
	{
		Portrait.Texture = image;
		Color = color;
		Portrait.Modulate = this.Color;
	}

	public void SetHealth(int current, int max)
	{
		healthDisplay.Text = $"{current}/{max}";
		
		if (current <= 0)
		{
			//TODO Should dead characters have a death portrait?
			healthDisplay.Text = "";
			Portrait.Modulate = deathColor;
		}
	}
}

