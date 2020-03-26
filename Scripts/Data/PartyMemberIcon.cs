using Godot;
using System;

public struct PartyMemberIcon
{
    public ReferenceRect _rect;
    public TextureRect _portrait;
    public Label _healthDisplay;
    public Color Color { get; set; }

    public void Highlight(bool isSelected)
    {
        if (isSelected)
            _portrait.Modulate = this.Color.Lightened(1);
        else
            _portrait.Modulate = this.Color;
    }

    public void SetHealth(int current, int max = -1)
    {
        if (max < -1)
            _healthDisplay.Text = $"{current}/{max}";
        else
            _healthDisplay.Text = $"{ current }/{_healthDisplay.Text.Split('/')[1]}";
    }

    public PartyMemberIcon(ReferenceRect rect, TextureRect portrait, Label healthDisplay, Color color)
    {
        _rect = rect;
        _portrait = portrait;
        _healthDisplay = healthDisplay;
        Color = color;

        _portrait.Modulate = Color;
    }
}

