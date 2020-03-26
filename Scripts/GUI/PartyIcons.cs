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
        GUIManager.RegisterElement(this);

        var party = GetChildren();
        partyMembers = new PartyMemberIcon[Math.Min(party.Count, maxPartyMembers)];

        for (int i = 0; i < partyMembers.Length; i++)
        {
            var p = (ReferenceRect)party[i];

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
        //Pass click up to manager
        //Written this way to make use of Godot 'signals'
        GUIManager.ClickPortrait(partyMember);
    }

    public void SetPortraitSelected(int partyMemberIndex, bool activeStatus, bool individualSelect = false)
    {
        if (partyMemberIndex <= partyMembers.Length)
        {
            if (individualSelect)
                foreach (var p in partyMembers)
                    p.Highlight(false);

            partyMembers[partyMemberIndex - 1].Highlight(activeStatus);
        }
    }
}
