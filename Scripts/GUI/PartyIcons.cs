using Godot;
using System;

public class PartyIcons : VBoxContainer
{
    [Export] public readonly int maxPartyMembers = 3;
    int currentPartyMembers;
    public PartyMemberIcon[] partyMembers;

    [Export] public Color[] partyColors;

    public override void _Ready()
    {
        var party = GetChildren();
        partyMembers = new PartyMemberIcon[Math.Min(party.Count, maxPartyMembers)];

        for (int i = 0; i < partyMembers.Length; i++)
        {
            var p = (ReferenceRect)party[i];
            GD.Print(p);

            var pmi = new PartyMemberIcon(
                p,
                (TextureRect)p.GetNode("Portrait"),
                (Label)p.GetNode("HealthDisplay"),
                partyColors[i]
            );

            partyMembers[i] = pmi;
        }
    }

    private void ClickPortrait(object @event, int partyMember)
    {
        //if mouse button RIGHT or SHIFT held, add to selection
        //if regular mouse, set to active
        if (Input.IsActionPressed("modify") )
        {
            GD.Print("MOD");

            if (Input.IsMouseButtonPressed(1))
                MapCharacterManager.AddCharacterToSelected(partyMember);
            else if (Input.IsMouseButtonPressed(2))
                MapCharacterManager.DeselectPartyMember(partyMember);
        }

        if (Input.IsMouseButtonPressed(1))
            MapCharacterManager.SelectPartyMember(partyMember);


    }


}

public struct PartyMemberIcon
{
    public ReferenceRect _rect;
    public TextureRect _portrait;
    public Label _healthDisplay;
    public Color Color
    {
        get
        {
            return _portrait.GetModulate();
        }
        set
        {
            _portrait.Modulate = value;
        }
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
    }
}

