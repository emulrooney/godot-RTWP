using Godot;
using System;

/// <summary>
/// Class to handle party members. Contains methods 
/// </summary>
public class PartyManager : Node
{
	public bool Loaded { get; private set; } = false;

	private static PackedScene template;
	private int membersInParty;

	public PartyMemberData[] PartyMembers { get; set; } = new PartyMemberData[6]; 
	public PlayerCharacter[] MapCharacters { get; set; } = new PlayerCharacter[6];

	public override void _Ready()
	{
		template =  ResourceLoader.Load("res://Characters/TEMPLATE_PlayerCharacter.tscn") as PackedScene;

		//For debug only. Probably won't bother with this method later.
		//LoadPartyData();
		//GenerateLocalPlayerCharacters();
	}

	public void LoadPartyData()
	{
		membersInParty = GetChildCount();

		for (int i = 0; i < membersInParty && i < 6; i++)
		{
			PartyMembers[i] = GetChild<PartyMemberData>(i);
		}

		Loaded = true;
	}

	public void UpdatePartyData()
	{
		for (int i = 0; i < MapCharacters.Length; i++)
		{
			if (MapCharacters[i] != null && PartyMembers[i] != null)
			{
				PartyMembers[i].UpdateFrom(MapCharacters[i]);
			}
			else
				return;
		}
	}

	public PlayerCharacter[] GenerateLocalPlayerCharacters()
	{
		MapCharacters = new PlayerCharacter[6];

		for (int i = 0; i < membersInParty; i++)
		{
			var generated = (PlayerCharacter)template.Instance();
			generated.LoadData(PartyMembers[i]);

			MapCharacters[i] = generated;
		}

		return MapCharacters;
	}
}
