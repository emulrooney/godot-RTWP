using Godot;
using System;

public class PartyMemberIcon : ReferenceRect
{
    public bool IsUsed { get; set; } = false;
    public TextureRect Portrait { get; set; }
    public Label healthDisplay;
    public Color Color { get; set; }

    private Color deathColor = new Color(0.3f, 0.3f, 0.3f);

    public override void _Ready()
    {
        Portrait = (TextureRect)GetNode("Portrait");
        healthDisplay = (Label)GetNode("HealthDisplay");
        IsUsed = false;
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

    public void SetHealth(int current, int max = -1)
    {
        if (max > -1)
            healthDisplay.Text = $"{current}/{max}";
        else
            healthDisplay.Text = $"{ current }/{healthDisplay.Text.Split('/')[1]}";

        if (current <= 0)
        {
            //TODO Should dead characters have a death portrait?
            this.Color = deathColor;
        }
    }
}

