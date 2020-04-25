using Godot;
using System;

public class PartyMemberData : Node
{
	//Character
	[Export] public string CharacterName { get; set; }
	[Export] public Texture Portrait { get; set; } //TODO see spriteFrames
	[Export] public Color PortraitColor { get; set; }
	[Export] public SpriteFrames AnimationSet { get; set; } //TODO Store in an array, access w int; needed for serialization

	//Statblock
	[Export] public int CurrentHP { get; set; } 
	[Export] public int MaxHP { get; set; }
	[Export] public float MoveSpeed { get; set; }
	[Export] public int BaseAccuracy { get; set; }
	[Export] public int BaseDamage { get; set; }
	[Export] public int BaseDefense { get; set; }

    /// <summary>
    /// Update from playerCharacter on map. Only update things that actually change; ie "Current" values.
    /// </summary>
    /// <param name="pc">Player to base new stats on</param>
    public void UpdateFrom(PlayerCharacter pc)
    {
        GD.Print("Updating " + CharacterName + " with HP --> " + CurrentHP);
        CurrentHP = pc.Stats.CurrentHP;
    }

}
