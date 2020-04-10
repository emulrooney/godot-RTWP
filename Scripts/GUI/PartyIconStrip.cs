using Godot;
using System;
using System.Linq;

public class PartyIconStrip : VBoxContainer
{

	[Export] public readonly int maxPartyMembers = 4;
	public PartyMemberIcon[] partyMembers;

	public override void _Ready()
	{
		GUIManager.RegisterElement(this);
	}

	public void Initialize()
	{
		partyMembers = GetChildren().OfType<PartyMemberIcon>().ToArray(); //convert to real array
	}

	public void SetupCharacterPortrait(PlayerCharacter character)
	{
		int portraitIndex = GetFirstFreePortraitIndex();

		if (portraitIndex == -1)
			return; //Exit early due to no free portraits

		var pmi = partyMembers[portraitIndex];

		try
		{
            pmi.SetPortrait(character.Portrait, character.PortraitColor);
            pmi.SetHealth(character.Stats.CurrentHP, character.Stats.MaxHP);
            pmi.IsUsed = true;
            pmi.Visible = true;
            pmi.IconFor = character;
        }
		catch (Exception e)
		{
			GD.Print($"Couldn't set portrait up for {character.CharacterName} - {character}");
            GD.Print("Exception: " + e);
		}
	}

    public void UpdateFor(PlayerCharacter character)
    {
        for (int i = 0; i < partyMembers.Length; i++)
        {
            if (partyMembers[i].IconFor == character)
            {
                partyMembers[i].SetHealth(character.Stats.CurrentHP, character.Stats.MaxHP);
                return;
            }
        }
    }

    private int GetFirstFreePortraitIndex()
	{
		for (int i = 0; i < partyMembers.Length; i++)
			if (!partyMembers[i].IsUsed)
				return i;

		return -1;
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

    public void WipeAllPortraits()
    {
        foreach (PartyMemberIcon pmi in partyMembers)
            pmi.Reset();
    }
}
